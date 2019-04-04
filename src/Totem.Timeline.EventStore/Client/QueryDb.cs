using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Client;
using Totem.Timeline.EventStore.DbOperations;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// Monitors changed queries via the changed-queries stream
  /// </summary>
  public sealed class QueryDb : IQueryDb
  {
    readonly ConcurrentDictionary<Id, QueryConnection> _connectionsById = new ConcurrentDictionary<Id, QueryConnection>();
    readonly ConcurrentDictionary<FlowKey, QueryInstance> _instancesByKey = new ConcurrentDictionary<FlowKey, QueryInstance>();
    readonly EventStoreContext _context;
    readonly IQueryNotifier _notifier;
    readonly QueryChangedSubscription _changedSubscription;
    readonly QueryChangedWindow _changedWindow;

    public QueryDb(EventStoreContext context, IQueryNotifier notifier)
    {
      _context = context;
      _notifier = notifier;
      _changedSubscription = new QueryChangedSubscription(this, context);
      _changedWindow = new QueryChangedWindow();
    }

    public async Task<QueryState> ReadState(QueryETag etag)
    {
      var stream = etag.Key.GetCheckpointStream();

      var result = await _context.Connection.ReadEventAsync(stream, StreamPosition.End, resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return new QueryState(etag.WithoutCheckpoint(), GetDefaultData(etag));
        case EventReadStatus.Success:
          var number = result.Event?.Event.EventNumber;
          var data = result.Event?.Event.Data;

          var checkpoint = new TimelinePosition(number);

          return checkpoint == etag.Checkpoint
            ? new QueryState(etag)
            : new QueryState(etag.WithCheckpoint(checkpoint), new MemoryStream(data));
        default:
          throw new Exception($"Unexpected result when reading {stream}: {result.Status}");
      }
    }

    public async Task<Stream> ReadContent(QueryETag etag) =>
      etag.Checkpoint.IsNone
        ? GetDefaultData(etag)
        : await new ReadQueryContentCommand(_context, etag).Execute();

    Stream GetDefaultData(QueryETag etag) =>
      new MemoryStream(_context.Json.ToJsonUtf8(etag.Key.Type.New()));

    //
    // Subscriptions
    //

    public async Task SubscribeToChanged(Id connectionId, QueryETag etag)
    {
      var connection = _connectionsById.GetOrAdd(connectionId, _ => new QueryConnection(connectionId, this));
      var instance = _instancesByKey.GetOrAdd(etag.Key, _ => new QueryInstance(etag.Key, this));

      connection.Subscribe(instance);

      await _changedSubscription.EnsureSubscribed();

      var latestETag = _changedWindow.OnSubscribed(etag);

      if(latestETag != etag)
      {
        await _notifier.NotifyChanged(latestETag, Many.Of(connectionId));
      }
    }

    public void UnsubscribeFromChanged(Id connectionId, FlowKey key)
    {
      if(_connectionsById.TryGetValue(connectionId, out var connection))
      {
        connection.Unsubscribe(key);
      }
    }

    public void UnsubscribeFromChanged(Id connectionId)
    {
      if(_connectionsById.TryGetValue(connectionId, out var connection))
      {
        connection.Unsubscribe();
      }
    }

    internal void OnChanged(QueryETag etag)
    {
      if(_instancesByKey.TryGetValue(etag.Key, out var instance))
      {
        _notifier.NotifyChanged(etag, instance.ConnectionIds);
      }

      _changedWindow.OnChanged(etag);
    }

    internal void RemoveConnection(QueryConnection connection)
    {
      _connectionsById.TryRemove(connection.Id, out var _);

      UnsubscribeIfIdle();
    }

    internal void RemoveInstance(QueryInstance instance)
    {
      _instancesByKey.TryRemove(instance.Key, out var _);

      UnsubscribeIfIdle();
    }

    void UnsubscribeIfIdle()
    {
      if(_connectionsById.Count == 0 && _instancesByKey.Count == 0)
      {
        _changedSubscription.Unsubscribe();
      }
    }
  }
}