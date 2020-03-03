using System;
using System.IO;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Timeline.Client;

namespace Totem.Timeline.StreamsDb.Client
{
  /// <summary>
  /// An EventStore database containing timeline data relevant to clients
  /// </summary>
  public sealed class ClientDb : Connection, IClientDb
  {
    readonly StreamsDbContext _context;

    public ClientDb(StreamsDbContext context)
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

      var nextExpectedVersion = await _context.AppendToTimelineAsync(data) + 1;

      return new TimelinePosition(nextExpectedVersion);
    }

    public Task<QueryState> ReadQuery(QueryETag etag) =>
      ReadQueryCheckpoint(etag.Key, () => GetDefaultState(etag), e => GetCheckpointState(etag, e));

    public Task<Query> ReadQueryContent(FlowKey key) =>
      ReadQueryCheckpoint(key, () => GetDefaultContent(key), e => GetCheckpointContent(key, e));

    async Task<TResult> ReadQueryCheckpoint<TResult>(FlowKey key, Func<TResult> getDefault, Func<Message, TResult> getCheckpoint)
    {
      var stream = key.GetCheckpointStream(_context.AreaName);

      var (message, found) = await _context.Client.DB().ReadLastMessageFromStream(stream);

      if (!found)
      {
        return getDefault();
      }
        
      return getCheckpoint(message);
    }

    QueryState GetDefaultState(QueryETag etag)
    {
      var defaultJson = _context.Json.ToJsonUtf8(etag.Key.Type.New());

      return new QueryState(etag.WithoutCheckpoint(), new MemoryStream(defaultJson));
    }

    QueryState GetCheckpointState(QueryETag etag, Message e)
    {
      var metadata = _context.ReadCheckpointMetadata(e);

      if(metadata.ErrorPosition.IsSome)
      {
        throw new Exception($"Query is stopped at {metadata.ErrorPosition} with the following error: {metadata.ErrorMessage}");
      }

      var checkpoint = new TimelinePosition(e.Position);

      return checkpoint == etag.Checkpoint
        ? new QueryState(etag)
        : new QueryState(etag.WithCheckpoint(checkpoint), new MemoryStream(e.Value));
    }

    Query GetDefaultContent(FlowKey key) =>
      (Query) key.Type.New();

    Query GetCheckpointContent(FlowKey key, Message e)
    {
      var metadata = _context.ReadCheckpointMetadata(e);

      if(metadata.ErrorPosition.IsSome)
      {
        throw new Exception($"Query is stopped at {metadata.ErrorPosition} with the following error: {metadata.ErrorMessage}");
      }

      return (Query) _context.Json.FromJsonUtf8(e.Value, key.Type.DeclaredType);
    }
  }
}