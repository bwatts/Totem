using System;
using System.IO;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Timeline.Client;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// An EventStore database containing timeline data relevant to clients
  /// </summary>
  public sealed class ClientDb : Connection, IClientDb
  {
    readonly EventStoreContext _context;

    public ClientDb(EventStoreContext context)
    {
      _context = context;
    }

    protected override Task Open()
    {
      // This is a slight hack.
      //
      // Normally this class is mutually exclusive with TimelineDb, which runs the timeline.
      // Both assume they are the top-level component, and connect to EventStore.
      //
      // However, the tests in Totem.Timeline.IntegrationTests need to host the timeline
      // while also acting as a client. This class and TimelineDb are both present in that
      // case, so do not attempt to connect to EventStore if TimelineDb already did.

      if(_context.State.IsDisconnected)
      {
        Track(_context);
      }

      return base.Open();
    }

    public async Task<IDisposable> Subscribe(IClientObserver observer)
    {
      var subscription = new ClientSubscription(_context, observer);

      await subscription.Subscribe();

      return subscription;
    }

    public async Task<TimelinePosition> WriteEvent(Event e)
    {
      var type = _context.GetEventType(e);

      var data = _context.GetAreaEventData(
        e,
        TimelinePosition.None,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        false,
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        null,
        type.GetRoutes(e).ToMany());

      var result = await _context.AppendToTimeline(data);

      return new TimelinePosition(result.NextExpectedVersion);
    }

    public async Task<Query> ReadQuery(FlowKey key)
    {
      var query = await ReadQueryCheckpoint(key, () => GetDefaultContent(key), e => GetCheckpointContent(key, e));

      FlowContext.Bind(query, key);

      return query;
    }

    public Task<QueryContent> ReadQueryContent(QueryETag etag) =>
      ReadQueryCheckpoint(etag.Key, () => GetDefaultContent(etag), e => GetCheckpointContent(etag, e));

    async Task<TResult> ReadQueryCheckpoint<TResult>(FlowKey key, Func<TResult> getDefault, Func<ResolvedEvent, TResult> getCheckpoint)
    {
      var stream = key.GetCheckpointStream();

      var result = await _context.Connection.ReadEventAsync(stream, StreamPosition.End, resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return getDefault();
        case EventReadStatus.Success:
          return getCheckpoint(result.Event.Value);
        default:
          throw new Exception($"Unexpected result when reading {stream}: {result.Status}");
      }
    }

    Query GetDefaultContent(FlowKey key) =>
      (Query) key.Type.New();

    Query GetCheckpointContent(FlowKey key, ResolvedEvent e)
    {
      var metadata = _context.ReadCheckpointMetadata(e);

      if(metadata.ErrorPosition.IsSome)
      {
        throw new Exception($"Query is stopped at {metadata.ErrorPosition} with the following error: {metadata.ErrorMessage}");
      }

      return (Query) _context.Json.FromJsonUtf8(e.Event.Data, key.Type.DeclaredType);
    }

    QueryContent GetDefaultContent(QueryETag etag)
    {
      var defaultJson = _context.Json.ToJsonUtf8(etag.Key.Type.New());

      return new QueryContent(etag.WithoutCheckpoint(), new MemoryStream(defaultJson));
    }

    QueryContent GetCheckpointContent(QueryETag etag, ResolvedEvent e)
    {
      var metadata = _context.ReadCheckpointMetadata(e);

      if(metadata.ErrorPosition.IsSome)
      {
        throw new Exception($"Query is stopped at {metadata.ErrorPosition} with the following error: {metadata.ErrorMessage}");
      }

      var checkpoint = new TimelinePosition(e.Event.EventNumber);

      return checkpoint == etag.Checkpoint
        ? new QueryContent(etag)
        : new QueryContent(etag.WithCheckpoint(checkpoint), new MemoryStream(e.Event.Data));
    }
  }
}