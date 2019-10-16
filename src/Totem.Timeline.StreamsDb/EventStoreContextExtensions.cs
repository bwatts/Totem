using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime.Json;
using Totem.Timeline.Area;

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

    internal static MessageInput GetAreaEventData(
      this EventStoreContext context,
      Event e,
      TimelinePosition cause,
      DateTimeOffset when,
      DateTimeOffset? whenOccurs,
      bool fromSchedule,
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
        FromSchedule = fromSchedule,
        CommandId = commandId,
        UserId = userId,
        Topic = topic
      };

      metadata.ApplyRoutes(type, routes);

      return new MessageInput
      {
        ID = eventId.IsUnassigned ? Guid.NewGuid().ToString() : eventId.ToString(),
        Type = type.ToString(),
        Value = context.ToJson(e),
        Header = context.ToJson(metadata)
      };
    }

    internal static Many<MessageInput> GetNewEventData(
      this EventStoreContext context,
      TimelinePosition cause,
      FlowKey topic,
      Many<Event> newEvents) =>

      newEvents.ToMany(e => context.GetAreaEventData(
        e,
        cause,
        e.When,
        Event.Traits.WhenOccurs.Get(e),
        false,
        Event.Traits.EventId.Get(e),
        Event.Traits.CommandId.Get(e),
        Event.Traits.UserId.Get(e),
        topic,
        context.GetEventType(e).GetRoutes(e).ToMany()));

    internal static MessageInput GetScheduledEventData(this EventStoreContext context, TimelinePoint cause, DateTimeOffset now) =>
      context.GetAreaEventData(
        cause.Event,
        cause.Position,
        now,
        null,
        true,
        Id.FromGuid(),
        cause.CommandId,
        cause.UserId,
        null,
        cause.Type.GetRoutes(cause.Event).ToMany());

    internal static MessageInput GetCheckpointEventData(this EventStoreContext context, Flow flow) =>
      new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = "timeline:Checkpoint",
        Value = context.ToJson(flow),
        Header = context.ToJson(new CheckpointMetadata
        {
          Position = flow.Context.CheckpointPosition,
          ErrorPosition = flow.Context.ErrorPosition,
          ErrorMessage = flow.Context.ErrorMessage,
          IsDone = flow.Context.IsDone
        })
      };

    internal static MessageInput GetClientEventData(this EventStoreContext context, Event e) =>
      new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = $"timeline:{e.GetType().Name}",
        Value = context.ToJson(e),
        Header = null
      };

    //
    // Reads
    //

    internal static TimelinePoint ReadAreaPoint(this EventStoreContext context, Message e, AreaEventMetadata metadata)
    {
      var type = context.ReadEventType(e);

      return new TimelinePoint(
        new TimelinePosition(e.Position),
        metadata.Cause,
        type,
        metadata.When,
        metadata.WhenOccurs,
        Id.From($"{e.Stream}-{e.Position}"),
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

    internal static TimelinePoint ReadAreaPoint(this EventStoreContext context, Message e) =>
      context.ReadAreaPoint(e, context.ReadAreaMetadata(e));

    internal static AreaEventMetadata ReadAreaMetadata(this EventStoreContext context, Message e) =>
      context.Json.FromJsonUtf8<AreaEventMetadata>(e.Header);

    internal static CheckpointMetadata ReadCheckpointMetadata(this EventStoreContext context, Message e) =>
      context.Json.FromJsonUtf8<CheckpointMetadata>(e.Header);

    internal static EventType ReadEventType(this EventStoreContext context, Message e) =>
      context.Area.Events.Get(AreaTypeName.From(e.Type));

    static Event ReadEvent(this EventStoreContext context, Message e, EventType type) =>
      (Event)context.Json.FromJsonUtf8(e.Value, type.DeclaredType);

    //
    // Appends
    //

    static async Task<long> AppendEvent(this EventStoreContext context, string stream, MessageInput data)
    {
      var firstWrittenMessage = await context.Client.DB().AppendStream(stream, data);
      return firstWrittenMessage + 1;
    }


    internal static async Task<long> AppendToTimeline(this EventStoreContext context, IEnumerable<MessageInput> data)
    {
      var firstWrittenMessage = await context.Client.DB().AppendStream(TimelineStreams.Timeline, data);
      return firstWrittenMessage + data.Count();
    }

    internal static Task<long> AppendToTimelineAsync(this EventStoreContext context, MessageInput data) => context.AppendEvent(TimelineStreams.Timeline, data);

    internal static Task<long> AppendToCheckpoint(this EventStoreContext context, Flow flow) =>
      context.AppendEvent(flow.Context.Key.GetCheckpointStream(), context.GetCheckpointEventData(flow));

    internal static async Task<long> AppendToClient(this EventStoreContext context, Event e)
    {
      var firstWrittenMessage = await context.Client.DB().AppendStream(TimelineStreams.Client, context.GetClientEventData(e));
      return firstWrittenMessage + 1;
    }      

    //
    // Metadata
    //

    //internal static Task<long> SetCheckpointStreamMetadata(this EventStoreContext context, Flow flow, StreamMetadata value) =>
    //  context.Client.SetStreamMetadataAsync(flow.Context.Key.GetCheckpointStream(), ExpectedVersion.Any, value);
  }
}