using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Client;
using Totem.Timeline.StreamsDb.Client;
using Totem.Timeline.StreamsDb.DbOperations;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// An EventStore database containing an area's events and flows
  /// </summary>
  public sealed class TimelineDb : Connection, ITimelineDb
  {
    readonly StreamsDbContext _context;
    readonly IResumeProjection _resumeProjection;

    public TimelineDb(StreamsDbContext context, IResumeProjection resumeProjection)
    {
      _context = context;
      _resumeProjection = resumeProjection;
    }

    protected override async Task Open()
    {
      await _context.Connect(this);
      await _resumeProjection.SynchronizeAsync();
    }

    public Task<ResumeInfo> Subscribe(ITimelineObserver observer) =>
      new SubscribeCommand(_context, observer).Execute();

    public Task<FlowInfo> ReadFlow(FlowKey key) =>
      new ReadFlowCommand(_context, key).Execute();

    public Task<FlowResumeInfo> ReadFlowToResume(FlowKey key) =>
      new ReadFlowToResumeCommand(_context, key).Execute();

    public async Task<TimelinePosition> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents)
    {
      var data = _context.GetNewEventData(cause, topicKey, newEvents);

      var nextExpectedVersion = await _context.AppendToTimeline(data);

      return new TimelinePosition(nextExpectedVersion - newEvents.Count + 1);
    }

    public Task WriteScheduledEvent(TimelinePoint cause)
    {
      var data = _context.GetScheduledEventData(cause, Clock.Now);

      return _context.AppendToTimelineAsync(data);
    }

    public async Task WriteCheckpoint(Flow flow, TimelinePoint point)
    {
      var result = await _context.AppendToCheckpoint(flow);

      await TryWriteToClient(flow, point);
    }

    async Task TryWriteToClient(Flow flow, TimelinePoint point)
    {
      var isQuery = flow is Query;
      var isStopped = flow.Context.ErrorPosition.IsSome;

      if(isQuery && isStopped)
      {
        await TryWriteQueryStopped(flow);
      }
      else if(isQuery)
      {
        await TryWriteQueryChanged(flow);
      }
      else
      {
        if(isStopped && point.CommandId.IsAssigned)
        {
          await TryWriteCommandFailed(flow, point.CommandId);
        }
      }
    }

    async Task TryWriteQueryChanged(Flow query)
    {
      try
      {
        var etag = QueryETag.From(query.Context.Key, query.Context.CheckpointPosition).ToString();

        await _context.AppendToClient(new QueryChanged(etag));
      }
      catch(Exception error)
      {
        Log.Error(error, "Failed to write update of query {Query} to the client stream. Subscribers will not be aware of the change until the query observes another event.", query);
      }
    }

    async Task TryWriteQueryStopped(Flow flow)
    {
      try
      {
        var etag = QueryETag.From(flow.Context.Key, flow.Context.CheckpointPosition).ToString();

        await _context.AppendToClient(new QueryStopped(etag, flow.Context.ErrorMessage));
      }
      catch(Exception error)
      {
        Log.Error(error, "Failed to write stoppage of query {Query} to the client stream. Subscribers will not be aware of the failure", flow);
      }
    }

    async Task TryWriteCommandFailed(Flow topic, Id commandId)
    {
      try
      {
        await _context.AppendToClient(new CommandFailed(commandId, topic.Context.ErrorMessage));
      }
      catch(Exception error)
      {
        Log.Error(error, "Failed to write failure of command {CommandId} to the client stream. The pending request will not know about the error.", commandId);
      }
    }
  }
}