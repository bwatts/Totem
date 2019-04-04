using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Timeline.Area;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Writes a topic's new events to EventStore
  /// </summary>
  internal class WriteNewEventsCommand : Notion
  {
    readonly EventStoreContext _context;
    readonly TimelinePosition _cause;
    readonly FlowKey _topicKey;
    readonly Many<Event> _newEvents;
    readonly Many<EventType> _newEventTypes;
    TimelinePosition _newPosition;

    internal WriteNewEventsCommand(
      EventStoreContext context,
      TimelinePosition cause,
      FlowKey topicKey,
      Many<Event> newEvents)
    {
      _context = context;
      _cause = cause;
      _topicKey = topicKey;
      _newEvents = newEvents;

      _newEventTypes = _newEvents.ToMany(e => _context.GetEventType(e));
    }

    internal async Task<ImmediateGivens> Execute()
    {
      await WriteNewEvents();

      return new ImmediateGivens(ReadImmediateGivenCalls().ToMany());
    }

    async Task WriteNewEvents()
    {
      // EventStore was not happy when the enumerator failed - eagerly evaluate the event data

      var result = await _context.Connection.AppendToStreamAsync(
        TimelineStreams.Timeline,
        ExpectedVersion.Any,
        _context.GetNewEventData(_cause, _topicKey, _newEvents).ToMany());

      _newPosition = new TimelinePosition(result.NextExpectedVersion - _newEvents.Count + 1);
    }

    IEnumerable<FlowCall.Given> ReadImmediateGivenCalls()
    {
      for(var i = 0; i < _newEvents.Count; i++)
      {
        var e = _newEvents[i];
        var type = _newEventTypes[i];

        if(TryReadImmediateGiven(e, type, out var call))
        {
          yield return call;
        }

        _newPosition = _newPosition.Next();
      }
    }

    bool TryReadImmediateGiven(Event e, EventType type, out FlowCall.Given call)
    {
      call = null;

      var routes = type.GetRoutes(e).ToMany();

      if(routes.Contains(_topicKey)
        && TryGetObservation(type, out var observation)
        && observation.HasGiven(Event.IsScheduled(e)))
      {
        var point = new TimelinePoint(
          _newPosition,
          _cause,
          type,
          e.When,
          Event.Traits.WhenOccurs.Get(e),
          Event.Traits.EventId.Get(e),
          Event.Traits.CommandId.Get(e),
          Event.Traits.UserId.Get(e),
          _topicKey,
          routes,
          () => e);

        call = new FlowCall.Given(point, observation);
      }

      return call != null;
    }

    bool TryGetObservation(EventType type, out TopicObservation observation)
    {
      observation = _topicKey.Type.Observations.TryGet(type, out var baseObservation)
        ? (TopicObservation) baseObservation
        : null;

      return observation != null;
    }
  }
}