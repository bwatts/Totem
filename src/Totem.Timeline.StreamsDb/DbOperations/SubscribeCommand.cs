using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb.DbOperations
{
  /// <summary>
  /// Subscribes to the timeline of the hosted area
  /// </summary>
  internal class SubscribeCommand
  {
    readonly StreamsDbContext _context;
    readonly ITimelineObserver _observer;

    internal SubscribeCommand(
      StreamsDbContext context,
      ITimelineObserver observer)
    {
      _context = context;
      _observer = observer;
    }

    internal async Task<ResumeInfo> Execute()
    {
      var (message, found) = await _context.Client.DB().ReadLastMessageFromStream($"{_context.AreaName}-{TimelineStreams.Resume}");

      if (!found)
      {
        return ReadInitialResumeInfo();
      }

      return await ReadResumeInfo(message.Value);
    }

    ResumeInfo ReadInitialResumeInfo() =>
      new ResumeInfo(new TimelineSubscription(_context, TimelinePosition.None, _observer));

    async Task<ResumeInfo> ReadResumeInfo(byte[] data)
    {
      var json = _context.Json.ToJObjectUtf8(data);

      var checkpoint = ReadCheckpoint(json["checkpoints"].Value<JObject>(), $"{_context.AreaName}-{TimelineStreams.Timeline}");
      var routes = ReadResumeFlows(json["routes"].Value<JArray>()).ToMany();
      var schedule = await ReadResumeSchedule(json["schedule"].Value<JArray>());

      var subscription = new TimelineSubscription(_context, checkpoint, _observer);

      return new ResumeInfo(checkpoint, routes, schedule, subscription);
    }

    TimelinePosition ReadCheckpoint(JObject json, string stream)
    {
      if (json.Type != JTokenType.Null)
      {
        foreach (JProperty prop in json.Properties())
        {
          if (prop.Name == stream)
          {
            return new TimelinePosition(prop.Value.Value<long>());
          }
        }
      }   

      return TimelinePosition.None;
    }

    IEnumerable<FlowKey> ReadResumeFlows(JArray json)
    {
      foreach(var typeItem in json)
      {
        if(typeItem is JArray multiInstance)
        {
          var type = _context.Area.GetFlow(AreaTypeName.From(multiInstance[0].Value<string>()));

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