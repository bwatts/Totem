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
    readonly QueryType _queryType;
    QueryApp _app;
    ITimelineObserver _observer;
    TimelinePosition _currentPosition;

    public QueryAppTimelineDb(AreaMap area, QueryType queryType)
    {
      _area = area;
      _queryType = queryType;
    }

    internal void SubscribeApp(QueryApp app) =>
      _app = app;

    public Task<ResumeInfo> Subscribe(ITimelineObserver observer)
    {
      _observer = observer;

      return Task.FromResult(new ResumeInfo(None));
    }

    public Task<FlowInfo> ReadFlow(FlowKey key) =>
      Task.FromResult(new FlowInfo.NotFound() as FlowInfo);

    public Task<FlowResumeInfo> ReadFlowToResume(FlowKey key) =>
      throw new InvalidOperationException("The timeline does not resume during query tests");

    public Task WriteScheduledEvent(TimelinePoint cause) =>
      throw new InvalidOperationException("Scheduled events do not occur during query tests");

    public Task<TimelinePosition> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents) =>
      throw new InvalidOperationException("New events do not occur during query tests");

    public Task WriteCheckpoint(Flow flow, TimelinePoint point)
    {
      _app.OnCheckpoint((Query) flow);

      return Task.CompletedTask;
    }

    internal async Task<TimelinePoint> WriteFromApp(Event e)
    {
      var eventType = _area.Events[e.GetType()];

      _queryType.ExpectObserves(eventType);

      _currentPosition = _currentPosition.Next();

      var point = new TimelinePoint(
        _currentPosition,
        TimelinePosition.None,
        eventType,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        null,
        eventType.GetRoutes(e, Event.IsScheduled(e)).ToMany(),
        () => e);

      await _observer.OnNext(point);

      return point;
    }
  }
}