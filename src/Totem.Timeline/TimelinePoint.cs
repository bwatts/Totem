using System;
using Totem.Timeline.Area;

namespace Totem.Timeline
{
  /// <summary>
  /// An event at a position on an area timeline
  /// </summary>
  public class TimelinePoint
  {
    readonly Lazy<Event> _event;

    public TimelinePoint(
      TimelinePosition position,
      TimelinePosition cause,
      EventType type,
      DateTimeOffset when,
      DateTimeOffset? whenOccurs,
      Id eventId,
      Id commandId,
      Id userId,
      FlowKey topic,
      Many<FlowKey> routes,
      Func<Event> readEvent)
    {
      Position = position;
      Cause = cause;
      Type = type;
      When = when;
      WhenOccurs = whenOccurs;
      EventId = eventId;
      CommandId = commandId;
      UserId = userId;
      Topic = topic;
      Routes = routes;

      _event = new Lazy<Event>(readEvent);
    }

    public readonly TimelinePosition Position;
    public readonly TimelinePosition Cause;
    public readonly EventType Type;
    public readonly DateTimeOffset When;
    public readonly DateTimeOffset? WhenOccurs;
    public readonly Id EventId;
    public readonly Id CommandId;
    public readonly Id UserId;
    public readonly FlowKey Topic;
    public readonly Many<FlowKey> Routes;

    public Event Event => _event.Value;
    public bool Scheduled => WhenOccurs != null;

    public override string ToString() => $"{Position} {Type}";

    public bool IsImmediateGiven() =>
      Topic != null && Routes.Contains(Topic);
  }
}