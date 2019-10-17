using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamsDB.Driver;
using Totem.Runtime.Json;
using Totem.Timeline.Area;
using Totem.Timeline.Json;
using Totem.Timeline.Runtime;
using Totem.Timeline.StreamsDb.DbOperations;

namespace Totem.Timeline.StreamsDb
{

  public class ResumeAreaSubscriber
  {
    private readonly StreamsDbContext _context;
    private readonly ResumeProjectionState _state;
    private readonly Func<Task> _onProjectionChanged;

    public ResumeAreaSubscriber(StreamsDbContext context, ResumeProjectionState state, Func<Task> onProjectionChanged)
    {
      _context = context;
      _state = state;
      _onProjectionChanged = onProjectionChanged;
    }

    public async Task Start()
    {
      var (message, found) = await _context.Client.DB().ReadLastMessageFromStream($"{_context.AreaName}-{TimelineStreams.Resume}");

      long position = 0;

      if (found)
      {
        var json = _context.Json.ToJObjectUtf8(message.Value);
        var checkpoint = ReadCheckpoint(json["checkpoint"]);
        position = checkpoint.ToInt64();
      }

      var subscription = _context.Client.DB().SubscribeStream($"{_context.AreaName}-{TimelineStreams.Timeline}", position + 1);

      await Task.Run(async () =>
      {
        do
        {
          await subscription.MoveNext();
          await Observe(subscription.Current);
          await _onProjectionChanged();
        }
        while (true);
      });
    }

    TimelinePosition ReadCheckpoint(JToken json) =>
     json.Type == JTokenType.Null ? TimelinePosition.None : new TimelinePosition(json.Value<long>());

    private async Task Observe(Message message)
    {
      var areaEventMetadata = JsonConvert.DeserializeObject<AreaEventMetadata>(System.Text.Encoding.UTF8.GetString(message.Header),
        new TimelinePositionConverter(), new FlowKeyConverter(_context.Area));

      await UpdateArea(message, areaEventMetadata);
    }

    private async Task UpdateArea(Message message, AreaEventMetadata metadata)
    {
      _state.Checkpoint = message.Position;

      await UpdateRoutes(message, metadata);
      await UpdateSchedule(message, metadata);
    }

    //
    // Routes
    //

    private async Task UpdateRoutes(Message message, AreaEventMetadata metadata)
    {
      var types = metadata.RouteTypes;

      foreach(var routeId in metadata.RouteIds)
      {
        var type = routeId.Type;
        var ids = routeId.Ids;

        foreach(var id in ids)
        {
          await UpdateMultiInstanceRoute(message, types[type], id);
        }

        types[type] = null;
      }

      foreach(var type in types)
      {
        if (type != null)
        {
          await UpdateSingleInstanceRoute(message, type);
        }
      }
    }

    private async Task UpdateSingleInstanceRoute(Message message, AreaTypeName type)
    {
      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = message.Type,
        Value = message.Value,
        Header = message.Header
      };

      await _context.Client.DB().AppendStream($"{_context.AreaName}-{type}-routes", messageInput);

      ResumeProjectInstance instance;

      if (_state.SingleInstances.ContainsKey(type))
      {
        instance = _state.SingleInstances[type];
      }
      else
      {
        instance = new ResumeProjectInstance();
        _state.SingleInstances.Add(type, instance);
      }

      instance.Latest = _state.Checkpoint;
    }

    private async Task UpdateMultiInstanceRoute(Message message, AreaTypeName type, Id id)
    {
      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = message.Type,
        Value = message.Value,
        Header = message.Header
      };

      await _context.Client.DB().AppendStream($"{_context.AreaName}-{type}|{id}-routes", messageInput);

      Dictionary<Id, ResumeProjectInstance> instances;

      if (_state.MultiInstances.ContainsKey(type))
      {
        instances = _state.MultiInstances[type];
      }
      else
      {
        instances = new Dictionary<Id, ResumeProjectInstance>();
        _state.MultiInstances.Add(type, instances);
      }

      ResumeProjectInstance instance;

      if (instances.ContainsKey(id))
      {
        instance = instances[id];
      }
      else
      {
        instance = new ResumeProjectInstance();
        instances.Add(id, instance);
      }

      instance.Latest = _state.Checkpoint;
    }

    //
    // Schedule
    //

    private async Task UpdateSchedule(Message message, AreaEventMetadata metadata)
    {
      if (metadata.WhenOccurs.HasValue)
      {
        await AppendToSchedule(message);
      }
      else
      {
        if (metadata.Cause.IsSome && metadata.FromSchedule)
        {
          RemoveFromSchedule(message, metadata);
        }
      }
    }

    private async Task AppendToSchedule(Message message)
    {
      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = message.Type,
        Value = message.Value,
        Header = message.Header
      };

      await _context.Client.DB().AppendStream($"{_context.AreaName}-schedule", messageInput);

      _state.Schedule[_state.Checkpoint] = null;
    }

    private void RemoveFromSchedule(Message message, AreaEventMetadata metadata)
    {
      var cause = metadata.Cause.ToInt64();
      _state.Schedule.Remove(cause);
    }
  }
}