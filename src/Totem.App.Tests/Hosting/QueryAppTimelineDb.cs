using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline;
using Totem.Timeline.Area;
using Totem.Timeline.Runtime;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An in-memory database containing a query and its events
  /// </summary>
  internal sealed class QueryAppTimelineDb : Connection, ITimelineDb
  {
    readonly AreaMap _area;
    QueryApp _app;
    ITimelineObserver _observer;
    TimelinePosition _currentPosition;

    public QueryAppTimelineDb(AreaMap area)
    {
      _area = area;
    }

    internal void SubscribeApp(QueryApp app) =>
      _app = app;

    public Task<ResumeInfo> Subscribe(ITimelineObserver observer)
    {
      _observer = observer;

      return Task.FromResult(new ResumeInfo(None));
    }

    public Task<FlowResumeInfo> ReadFlowResumeInfo(FlowKey key) =>
      Task.FromResult(new FlowResumeInfo.NotFound() as FlowResumeInfo);

    public Task WriteScheduledEvent(TimelinePoint cause) =>
      throw new InvalidOperationException("Scheduled events do not occur during query tests");

    public Task<TimelinePosition> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents) =>
      throw new InvalidOperationException("New events do not occur during query tests");

    public Task WriteCheckpoint(Flow flow, TimelinePoint point)
    {
      _app.OnCheckpoint((Query) flow);

      return Task.CompletedTask;
    }

    internal async Task<TimelinePosition> WriteFromApp(Event e)
    {
      var type = _area.Events[e.GetType()];

      _currentPosition = _currentPosition.Next();

      await _observer.OnNext(new TimelinePoint(
        _currentPosition,
        TimelinePosition.None,
        type,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        null,
        type.GetRoutes(e, Event.IsScheduled(e)).ToMany(),
        () => e));

      return _currentPosition;
    }
  }
}