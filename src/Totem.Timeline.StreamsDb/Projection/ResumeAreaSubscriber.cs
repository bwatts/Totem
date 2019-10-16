using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamsDB.Driver;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{

  public class ResumeAreaSubscriber
  {
    private readonly StreamsDbContext _context;
    private readonly ResumeProjectionState _state;

    public ResumeAreaSubscriber(StreamsDbContext context)
    {
      _context = context;
      _state = new ResumeProjectionState();
    }

    public Task Start()
    {
      var subscription = _context.Client.DB().SubscribeStream($"{_context.AreaName}-{TimelineStreams.Timeline}", 0);

      return Task.Run(async () =>
      {
        do
        {
          var hasNext = await subscription.MoveNext();

          if (!hasNext)
          {
            await Task.Delay(1000);
            continue;
          }

          await Observe(subscription.Current);
        }
        while (true);
      });      
    }

    private async Task Observe(Message message)
    {
      try
      {
        var areaEventMetadata = JsonConvert.DeserializeObject<AreaEventMetadata>(System.Text.Encoding.UTF8.GetString(message.Header));
        await UpdateArea(message, areaEventMetadata);
      }
      catch (Exception ex)
      {
        throw;
      } 
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