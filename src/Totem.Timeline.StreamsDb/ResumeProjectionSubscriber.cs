using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamsDB.Driver;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  public class ResumeProjectionState
  {
    public long Checkpoint { get; set; }
    public Dictionary<long, ResumeProjectionSchedule> Schedule { get; set; }
    public Dictionary<AreaTypeName, ResumeProjectInstance> SingleInstances { get; set; }
    public Dictionary<AreaTypeName, Dictionary<Id, ResumeProjectInstance>> MultiInstances { get; set; }
  }

  public class ResumeProjectionSchedule
  {

  }

  public class ResumeState
  {
    public long Checkpoint { get; set; }
    public List<string> Routes { get; set; }
    public List<long> Schedule { get; set; }
  }

  public class ResumeProjectInstance
  {
    public long? Latest { get; set; }
    public long? Checkpoint { get; set; }
    public bool IsStopped { get; set; }
  }

  public class ResumeProjectionSubscriber
  {
    private readonly StreamsDbContext _context;
    private readonly ResumeProjectionState _state;

    private string _areaName;

    public ResumeProjectionSubscriber(StreamsDbContext context)
    {
      _context = context;
      _state = new ResumeProjectionState();
    }

    public Task Start(string areaName)
    {
      _areaName = areaName;
      var subscription = _context.Client.DB().SubscribeStream($"#{areaName}", 0);

      return Task.Run(async () =>
      {
        do
        {
          if (!await subscription.MoveNext())
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
      if (message.Stream.StartsWith("$", StringComparison.InvariantCultureIgnoreCase))
      {
        return;
      }
      else if (message.Stream.EndsWith("-timeline", StringComparison.InvariantCultureIgnoreCase))
      {
        var areaEventMetadata = JsonConvert.DeserializeObject<AreaEventMetadata>(System.Text.Encoding.UTF8.GetString(message.Header));
        await UpdateArea(message, areaEventMetadata);
      }
      else
      {
        if (message.Stream.EndsWith("-checkpoint", StringComparison.InvariantCultureIgnoreCase))
        {
          var checkpointMetadata = JsonConvert.DeserializeObject<CheckpointMetadata>(System.Text.Encoding.UTF8.GetString(message.Header));
          UpdateProgress(message, checkpointMetadata);
        }
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

      await _context.Client.DB().AppendStream($"{_areaName}-{type}-routes", messageInput);

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

      await _context.Client.DB().AppendStream($"{_areaName}-{type}|{id}-routes", messageInput);

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
        if (metadata.Cause != null && metadata.FromSchedule)
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

      await _context.Client.DB().AppendStream($"{_areaName}-schedule", messageInput);

      _state.Schedule[_state.Checkpoint] = null;
    }

    private void RemoveFromSchedule(Message message, AreaEventMetadata metadata)
    {
      var cause = metadata.Cause.ToInt64();
      _state.Schedule.Remove(cause);
    }

    //
    // Progress
    //

    private void UpdateProgress(Message message, CheckpointMetadata metadata)
    {
      var flowKey = message.Stream.Substring(0, message.Stream.Length - "-checkpoint".Length);
      var separatorIndex = flowKey.IndexOf("|");

      if (separatorIndex == -1)
      {
        UpdateSingleInstanceProgress(metadata, flowKey);
      }
      else
      {
        var type = flowKey.Substring(0, separatorIndex);
        var id = flowKey.Substring(separatorIndex + 1);

        UpdateMultiInstanceProgress(metadata, type, id);
      }
    }

    private void UpdateSingleInstanceProgress(CheckpointMetadata metadata, string type)
    {
      var areaTypeName = AreaTypeName.From(type);

      if (metadata.IsDone)
      {
        _state.SingleInstances.Remove(areaTypeName);
      }
      else
      {
        ResumeProjectInstance instance;

        if (_state.SingleInstances.ContainsKey(areaTypeName))
        {
          instance = _state.SingleInstances[areaTypeName];
        }
        else
        {
          instance = new ResumeProjectInstance();
          _state.SingleInstances.Add(areaTypeName, instance);
        }

        instance.Checkpoint = metadata.Position.ToInt64OrNull();
        instance.IsStopped = instance.IsStopped || metadata.ErrorPosition != null;
      }
    }

    private void UpdateMultiInstanceProgress(CheckpointMetadata metadata, string type, string id)
    {
      Dictionary<Id, ResumeProjectInstance> instances;

      var areaTypeName = AreaTypeName.From(type);
      if (_state.MultiInstances.ContainsKey(areaTypeName))
      {
        instances = _state.MultiInstances[areaTypeName];
      }
      else
      {
        instances = new Dictionary<Id, ResumeProjectInstance>();
        _state.MultiInstances.Add(areaTypeName, instances);
      }

      if (metadata.IsDone)
      {
        instances.Remove(Id.From(id));

        if (!instances.Any())
        {
          _state.MultiInstances.Remove(areaTypeName);
        }
      }
      else
      {
        ResumeProjectInstance instance;

        var theId = Id.From(id);

        if (instances.ContainsKey(theId))
        {
          instance = instances[theId];
        }
        else
        {
          instance = new ResumeProjectInstance();
          instances.Add(theId, instance);
        }

        instance.Checkpoint = metadata.Position.ToInt64OrNull();
        instance.IsStopped = instance.IsStopped || metadata.ErrorPosition != null;
      }
    }

    //
    // Resume state
    //

    private ResumeState GetResumeState(Message message)
    {
      var routes = BuildRoutes();
      var schedule = BuildSchedule();

      return new ResumeState
      {
        Checkpoint = _state.Checkpoint,
        Routes = routes,
        Schedule = schedule
      };
    }

    private List<string> BuildRoutes()
    {
      var routes = new List<string>();

      foreach (var instance in _state.SingleInstances)
      {
        var singleTypeRoutes = BuildSingleTypeRoutes(instance.Key, instance.Value);
        routes.AddRange(singleTypeRoutes);
      }

      foreach (var instance in _state.MultiInstances)
      {
        var multiTypeRoutes = BuildMultiTypeRoutes(instance.Key, instance.Value);
        routes.AddRange(multiTypeRoutes);
      }

      return routes;
    }

    private List<long> BuildSchedule()
    {
      return _state.Schedule.Keys.OrderBy(x => x).ToList();
    }

    private List<string> BuildSingleTypeRoutes(AreaTypeName type, ResumeProjectInstance instance)
    {
      var routes = new List<string>();

      if (IsResumable(instance))
      {
        routes.Add(type.ToString());
      }

      return routes;
    }

    private List<string> BuildMultiTypeRoutes(AreaTypeName type, Dictionary<Id, ResumeProjectInstance> instances)
    {
      var resumableIds = instances.Where(instance => IsResumable(instance.Value)).Select(instance => instance.Key);
      var routes = new List<string>();

      routes.Add(type.ToString());
      routes.AddRange(resumableIds.Select(id => id.ToString()));

      return routes;
    }

    private bool IsResumable(ResumeProjectInstance instance)
    {
      return !instance.IsStopped &&
        instance.Latest != null &&
        (instance.Checkpoint == null || instance.Checkpoint < instance.Latest);
    }
  }
}