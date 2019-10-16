using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamsDB.Driver;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  public class ResumeProgressSubscriber
  {
    private readonly StreamsDbContext _context;
    private readonly FlowType _flowType;
    private readonly ResumeProjectionState _state;

    public ResumeProgressSubscriber(StreamsDbContext context, FlowType flowType)
    {
      _context = context;
      _flowType = flowType;
      _state = new ResumeProjectionState();
    }

    public Task Start()
    {
      var subscription = _context.Client.DB().SubscribeStream($"{_context.AreaName}-{_flowType.Name}-checkpoint", 0);

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

          Observe(subscription.Current);
        }
        while (true);
      });      
    }

    private void Observe(Message message)
    {
      try
      {
        var checkpointMetadata = JsonConvert.DeserializeObject<CheckpointMetadata>(System.Text.Encoding.UTF8.GetString(message.Header));
        UpdateProgress(message, checkpointMetadata);
      }
      catch (Exception ex)
      {
        throw;
      }
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