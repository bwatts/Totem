using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// Hosts query subscriptions as a service in the .NET runtime
  /// </summary>
  public sealed class QueryHost : IQueryHost
  {
    readonly ConcurrentDictionary<Id, QueryConnection> _connectionsById = new ConcurrentDictionary<Id, QueryConnection>();
    readonly ConcurrentDictionary<FlowKey, QueryInstance> _instancesByKey = new ConcurrentDictionary<FlowKey, QueryInstance>();
    readonly QueryChangedWindow _changedWindow = new QueryChangedWindow();
    readonly IQueryNotifier _notifier;

    public QueryHost(IQueryNotifier notifier)
    {
      _notifier = notifier;
    }

    public async Task SubscribeToChanged(Id connectionId, QueryETag etag)
    {
      var connection = _connectionsById.GetOrAdd(connectionId, _ => new QueryConnection(connectionId, this));
      var instance = _instancesByKey.GetOrAdd(etag.Key, _ => new QueryInstance(etag.Key, this));

      connection.Subscribe(instance);

      var latestETag = _changedWindow.OnSubscribed(etag);

      if(latestETag != etag)
      {
        await _notifier.NotifyChanged(latestETag, Many.Of(connectionId));
      }
    }

    public Task UnsubscribeFromChanged(Id connectionId, FlowKey key)
    {
      if(_connectionsById.TryGetValue(connectionId, out var connection))
      {
        connection.Unsubscribe(key);
      }

      return Task.CompletedTask;
    }

    public Task UnsubscribeFromChanged(Id connectionId)
    {
      if(_connectionsById.TryGetValue(connectionId, out var connection))
      {
        connection.Unsubscribe();
      }

      return Task.CompletedTask;
    }

    internal async Task OnChanged(QueryETag etag)
    {
      if(_instancesByKey.TryGetValue(etag.Key, out var instance))
      {
        await _notifier.NotifyChanged(etag, instance.ConnectionIds);
      }

      _changedWindow.OnChanged(etag);
    }

    internal async Task OnStopped(QueryETag etag, string error)
    {
      if(_instancesByKey.TryGetValue(etag.Key, out var instance))
      {
        await _notifier.NotifyStopped(etag, error, instance.ConnectionIds);
      }

      _changedWindow.OnChanged(etag);
    }

    internal void RemoveConnection(QueryConnection connection) =>
      _connectionsById.TryRemove(connection.Id, out _);

    internal void RemoveInstance(QueryInstance instance) =>
      _instancesByKey.TryRemove(instance.Key, out _);
  }
}