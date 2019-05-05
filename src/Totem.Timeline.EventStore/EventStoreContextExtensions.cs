using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.EventStore.Client;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// Extends <see cref="EventStoreContext"/> with database operations
  /// </summary>
  internal static class EventStoreContextExtensions
  {
    internal static EventType GetEventType(this EventStoreContext context, Event e) =>
      context.Area.Events.Get(e.GetType());

    internal static byte[] ToJson(this EventStoreContext context, object value) =>
      value == null ? null : context.Json.ToJsonUtf8(value).ToBytes();

    //
    // Event data
    //

    internal static EventData GetAreaEventData(
      this EventStoreContext context,
      Event e,
      TimelinePosition cause,
      DateTimeOffset when,
      DateTimeOffset? whenOccurs,
      Id eventId,
      Id commandId,
      Id userId,
      FlowKey topic,
      Many<FlowKey> routes)
    {
      var type = context.GetEventType(e);

      var metadata = new AreaEventMetadata
      {
        Cause = cause,
        When = when,
        WhenOccurs = whenOccurs,
        CommandId = commandId,
        UserId = userId,
        Topic = topic
      };

      metadata.ApplyRoutes(type, routes);

      return new EventData(
        eventId.IsUnassigned ? Guid.NewGuid() : Guid.Parse(eventId.ToString()),
        type.ToString(),
        isJson: true,
        data: context.ToJson(e),
        metadata: context.ToJson(metadata));
    }

    internal static IEnumerable<EventData> GetNewEventData(
      this EventStoreContext context,
      TimelinePosition cause,
      FlowKey topic,
      Many<Event> newEvents) =>

      from e in newEvents
      select context.GetAreaEventData(
        e,
        cause,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        topic,
        context.GetEventType(e).GetRoutes(e).ToMany());

    internal static EventData GetCheckpointEventData(this EventStoreContext context, Flow flow) =>
      new EventData(
        Guid.NewGuid(),
        "timeline:Checkpoint",
        isJson: true,
        data: context.ToJson(flow),
        metadata: context.ToJson(new CheckpointMetadata
        {
          Position = flow.Context.CheckpointPosition,
          ErrorPosition = flow.Context.ErrorPosition
        }));

    internal static EventData GetQueryChangedEventData(this EventStoreContext context, QueryChanged e) =>
      new EventData(
        Guid.NewGuid(),
        "timeline:QueryChanged",
        isJson: true,
        data: context.ToJson(e),
        metadata: null);

    //
    // Reads
    //

    internal static TimelinePoint ReadAreaPoint(this EventStoreContext context, ResolvedEvent e, AreaEventMetadata metadata)
    {
      var type = context.ReadEventType(e);

      return new TimelinePoint(
        new TimelinePosition(e.Event.EventNumber),
        metadata.Cause,
        type,
        metadata.When,
        metadata.WhenOccurs,
        Id.From(e.Event.EventId),
        metadata.CommandId,
        metadata.UserId,
        metadata.Topic,
        metadata.GetRoutes(context.Area).ToMany(),
        () =>
        {
          var instance = context.ReadEvent(e, type);

          Event.Traits.When.Set(instance, metadata.When);
          Event.Traits.WhenOccurs.Set(instance, metadata.WhenOccurs);
          Event.Traits.CommandId.Set(instance, metadata.CommandId);
          Event.Traits.UserId.Set(instance, metadata.UserId);

          return instance;
        });
    }

    internal static TimelinePoint ReadAreaPoint(this EventStoreContext context, ResolvedEvent e) =>
      context.ReadAreaPoint(e, context.ReadAreaMetadata(e));

    internal static AreaEventMetadata ReadAreaMetadata(this EventStoreContext context, ResolvedEvent e) =>
      context.Json.FromJsonUtf8<AreaEventMetadata>(e.Event.Metadata);

    internal static EventType ReadEventType(this EventStoreContext context, ResolvedEvent e) =>
      context.Area.Events.Get(AreaTypeName.From(e.Event.EventType));

    static Event ReadEvent(this EventStoreContext context, ResolvedEvent e, EventType type) =>
      (Event) context.Json.FromJsonUtf8(e.Event.Data, type.DeclaredType);
  }
}