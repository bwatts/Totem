using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Timeline.Client;
using Totem.Timeline.EventStore.Client;
using Totem.Timeline.EventStore.DbOperations;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// An EventStore database containing an area's events and flows
  /// </summary>
  public sealed class TimelineDb : Connection, ITimelineDb
  {
    readonly EventStoreContext _context;
    readonly CatchUpSubscriptionSettings _subscriptionSettings;
    readonly IResumeProjection _resumeProjection;

    public TimelineDb(EventStoreContext context, CatchUpSubscriptionSettings subscriptionSettings, IResumeProjection resumeProjection)
    {
      _context = context;
      _subscriptionSettings = subscriptionSettings;
      _resumeProjection = resumeProjection;
    }

    protected override async Task Open()
    {
      Track(_context);

      await _resumeProjection.Synchronize();
    }

    public Task<ResumeInfo> Subscribe(ITimelineObserver observer) =>
      new SubscribeCommand(_context, _subscriptionSettings, observer).Execute();

    public async Task<FlowResumeInfo> ReadFlowResumeInfo(FlowKey key)
    {
      var stream = key.GetCheckpointStream();

      var result = await _context.Connection.ReadEventAsync(stream, StreamPosition.End, resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return await new ReadFlowWithoutCheckpointCommand(_context, key).Execute();
        case EventReadStatus.Success:
          return await new ReadFlowWithCheckpointCommand(_context, key, result.Event?.Event).Execute();
        default:
          throw new Exception($"Unexpected result when reading {stream} to resume: {result.Status}");
      }
    }

    public Task WriteScheduledEvent(TimelinePoint cause)
    {
      var e = cause.Event;

      var data = _context.GetAreaEventData(
        e,
        cause.Position,
        Clock.Now,
        null,
        Id.FromGuid(),
        cause.CommandId,
        cause.UserId,
        null,
        cause.Type.GetRoutes(e, scheduled: false).ToMany());

      return _context.Connection.AppendToStreamAsync(TimelineStreams.Timeline, ExpectedVersion.Any, data);
    }

    public Task<ImmediateGivens> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents) =>
      new WriteNewEventsCommand(_context, cause, topicKey, newEvents).Execute();

    public async Task WriteCheckpoint(Flow flow)
    {
      var result = await _context.Connection.AppendToStreamAsync(
        flow.Context.Key.GetCheckpointStream(),
        ExpectedVersion.Any,
        _context.GetCheckpointEventData(flow));

      if(flow is Query query)
      {
        try
        {
          var checkpoint = new TimelinePosition(result.NextExpectedVersion);
          var etag = QueryETag.From(query.Context.Key, checkpoint).ToString();

          await _context.Connection.AppendToStreamAsync(
            TimelineStreams.ChangedQueries,
            ExpectedVersion.Any,
            _context.GetQueryChangedEventData(new QueryChanged(etag)));
        }
        catch(Exception error)
        {
          Log.Error(error, "Failed to write QueryChanged for {Query} - subscribers will not be aware of the change until the query observes another event.", query);
        }
      }
    }
  }
}