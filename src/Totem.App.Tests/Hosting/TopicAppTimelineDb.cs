using System;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Timeline;
using Totem.Timeline.Area;
using Totem.Timeline.Runtime;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An in-memory database containing a topic and its events
  /// </summary>
  internal sealed class TopicAppTimelineDb : Connection, ITimelineDb
  {
    readonly AreaMap _area;
    readonly IJsonFormat _json;
    readonly TopicType _topicType;
    TopicApp _app;
    ITimelineObserver _observer;
    TimelinePosition _currentPosition;

    public TopicAppTimelineDb(AreaMap area, IJsonFormat json, TopicType topicType)
    {
      _area = area;
      _json = json;
      _topicType = topicType;
    }

    internal void SubscribeApp(TopicApp app) =>
      _app = app;

    public Task<ResumeInfo> Subscribe(ITimelineObserver observer)
    {
      _observer = observer;

      return Task.FromResult(new ResumeInfo(None));
    }

    public Task<FlowInfo> ReadFlow(FlowKey key) =>
      Task.FromResult(new FlowInfo.NotFound() as FlowInfo);

    public Task<FlowResumeInfo> ReadFlowToResume(FlowKey key) =>
      throw new InvalidOperationException("The timeline does not resume during topic tests");

    public async Task<TimelinePosition> WriteNewEvents(TimelinePosition cause, FlowKey topicKey, Many<Event> newEvents)
    {
      foreach(var newEvent in newEvents)
      {
        var (copy, type) = CopyEvent(newEvent);

        await OnNext(new TimelinePoint(
          AdvancePosition(),
          cause,
          type,
          copy.When,
          Event.Traits.WhenOccurs.Get(copy),
          Event.Traits.EventId.Get(copy),
          Event.Traits.CommandId.Get(copy),
          Event.Traits.UserId.Get(copy),
          topicKey,
          type.GetRoutes(copy).ToMany(),
          () => copy));
      }

      return new TimelinePosition(_currentPosition.ToInt64() - newEvents.Count);
    }

    public Task WriteScheduledEvent(TimelinePoint cause)
    {
      var (copy, type) = CopyEvent(cause.Event);

      Event.Traits.When.Set(copy, Clock.Now);
      Event.Traits.WhenOccurs.Set(copy, null);

      return OnNext(new TimelinePoint(
        AdvancePosition(),
        cause.Position,
        type,
        copy.When,
        Event.Traits.WhenOccurs.Get(copy),
        Event.Traits.EventId.Get(copy),
        Event.Traits.CommandId.Get(copy),
        Event.Traits.UserId.Get(copy),
        null,
        type.GetRoutes(copy).ToMany(),
        () => copy));
    }

    public Task WriteCheckpoint(Flow flow, TimelinePoint point)
    {
      _app.OnCheckpoint((Topic) flow);

      return Task.CompletedTask;
    }

    internal async Task<TimelinePoint> WriteFromApp(Event e)
    {
      var (copy, type) = CopyEvent(e);

      _topicType.ExpectObserves(type);

      var point = new TimelinePoint(
        AdvancePosition(),
        TimelinePosition.None,
        type,
        copy.When,
        Event.Traits.WhenOccurs.Get(copy),
        Event.Traits.EventId.Get(copy),
        Event.Traits.CommandId.Get(copy),
        Event.Traits.UserId.Get(copy),
        null,
        type.GetRoutes(copy, Event.IsScheduled(copy)).ToMany(),
        () => copy);

      await OnNext(point);

      return point;
    }

    //
    // Details
    //

    (Event, EventType) CopyEvent(Event e)
    {
      var type = _area.Events[e.GetType()];

      var json = _json.ToJsonUtf8(e);

      var copy = (Event) _json.FromJsonUtf8(json, type.DeclaredType);

      Event.Traits.When.Bind(e, copy);
      Event.Traits.WhenOccurs.Bind(e, copy);
      Event.Traits.EventId.Bind(e, copy);
      Event.Traits.EventId.Bind(e, copy);
      Event.Traits.CommandId.Bind(e, copy);
      Event.Traits.UserId.Bind(e, copy);

      return (copy, type);
    }

    TimelinePosition AdvancePosition() =>
      _currentPosition = _currentPosition.Next();

    Task OnNext(TimelinePoint point)
    {
      _app.OnNext(point);

      return _observer.OnNext(point);
    }
  }
}