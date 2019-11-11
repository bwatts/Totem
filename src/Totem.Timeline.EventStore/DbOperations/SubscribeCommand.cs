using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json.Linq;
using Totem.Reflection;
using Totem.Runtime.Json;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore.DbOperations
{
  /// <summary>
  /// Subscribes to the timeline of the hosted area
  /// </summary>
  internal class SubscribeCommand
  {
    readonly EventStoreContext _context;
    readonly ITimelineObserver _observer;
    readonly CatchUpSubscriptionSettings _settings;

    internal SubscribeCommand(
      EventStoreContext context,
      CatchUpSubscriptionSettings settings,
      ITimelineObserver observer)
    {
      _context = context;
      _settings = settings;
      _observer = observer;
    }

    internal async Task<ResumeInfo> Execute()
    {
      var result = await _context.Connection.ReadEventAsync(
        TimelineStreams.Resume,
        StreamPosition.End,
        resolveLinkTos: false);

      switch(result.Status)
      {
        case EventReadStatus.NoStream:
        case EventReadStatus.NotFound:
          return ReadInitialResumeInfo();
        case EventReadStatus.Success:
          return await ReadResumeInfo(result.Event?.Event.Data);
        default:
          throw new Exception($"Unexpected result when reading {TimelineStreams.Resume} to resume: {result.Status}");
      }
    }

    ResumeInfo ReadInitialResumeInfo() =>
      new ResumeInfo(new TimelineSubscription(_context, _settings, TimelinePosition.None, _observer));

    async Task<ResumeInfo> ReadResumeInfo(byte[] data)
    {
      var json = _context.Json.ToJObjectUtf8(data);

      var checkpoint = ReadCheckpoint(json["checkpoint"]);
      var routes = ReadResumeFlows(json["routes"].Value<JArray>()).ToMany();
      var schedule = await ReadResumeSchedule(json["schedule"].Value<JArray>());

      var subscription = new TimelineSubscription(_context, _settings, checkpoint, _observer);

      return new ResumeInfo(checkpoint, routes, schedule, subscription);
    }

    TimelinePosition ReadCheckpoint(JToken json) =>
      json.Type == JTokenType.Null ? TimelinePosition.None : new TimelinePosition(json.Value<long>());

    IEnumerable<FlowKey> ReadResumeFlows(JArray json)
    {
      foreach(var typeItem in json)
      {
        if(typeItem is JArray multiInstance)
        {
          var type = _context.Area.GetFlow(TypeName.From(multiInstance[0].Value<string>()));

          foreach(var idItem in multiInstance.Skip(1))
          {
            yield return FlowKey.From(type, Id.From(idItem.Value<string>()));
          }
        }
        else
        {
          yield return FlowKey.From(typeItem.Value<string>(), _context.Area);
        }
      }
    }

    async Task<Many<TimelinePoint>> ReadResumeSchedule(JArray json)
    {
      if(json.Count == 0)
      {
        return new Many<TimelinePoint>();
      }

      var schedule = json.Values<long>().ToMany();

      return await new ReadResumeScheduleCommand(_context, schedule).Execute();
    }
  }
}