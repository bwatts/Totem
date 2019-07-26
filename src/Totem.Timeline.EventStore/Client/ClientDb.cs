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
      Track(_context);

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
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        null,
        type.GetRoutes(e).ToMany());

      var result = await _context.AppendToTimeline(data);

      return new TimelinePosition(result.NextExpectedVersion);
    }

    public async Task<QueryState> ReadQuery(QueryETag etag)
    {
      var stream = etag.Key.GetCheckpointStream();

      var result = await _context.Connection.ReadEventAsync(stream, StreamPosition.End, resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return GetDefaultState(etag);
        case EventReadStatus.Success:
          return GetCheckpointState(etag, result.Event.Value);
        default:
          throw new Exception($"Unexpected result when reading {stream}: {result.Status}");
      }
    }

    QueryState GetDefaultState(QueryETag etag)
    {
      var defaultJson = _context.Json.ToJsonUtf8(etag.Key.Type.New());

      return new QueryState(etag.WithoutCheckpoint(), new MemoryStream(defaultJson));
    }

    QueryState GetCheckpointState(QueryETag etag, ResolvedEvent e)
    {
      var metadata = _context.ReadCheckpointMetadata(e);

      if(metadata.ErrorPosition.IsSome)
      {
        throw new Exception($"Query is stopped at {metadata.ErrorPosition} with the following error: {metadata.ErrorMessage}");
      }

      var checkpoint = new TimelinePosition(e.Event.EventNumber);

      return checkpoint == etag.Checkpoint
        ? new QueryState(etag)
        : new QueryState(etag.WithCheckpoint(checkpoint), new MemoryStream(e.Event.Data));
    }
  }
}