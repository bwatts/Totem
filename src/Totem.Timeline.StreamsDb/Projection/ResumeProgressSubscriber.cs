using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamsDB.Driver;
using Totem.Timeline.Area;
using Totem.Timeline.Json;

namespace Totem.Timeline.StreamsDb
{
  public class ResumeProgressSubscriber
  {
    private readonly StreamsDbContext _context;
    private readonly FlowType _flowType;
    private readonly ResumeProjectionState _state;
    private readonly Func<Task> _onProjectionChanged;

    public ResumeProgressSubscriber(StreamsDbContext context, ResumeProjectionState state, FlowType flowType, Func<Task> onProjectionChanged)
    {
      _context = context;
      _flowType = flowType;
      _state = state;
      _onProjectionChanged = onProjectionChanged;
    }

    public Task Start()
    {
      var subscription = _context.Client.DB().SubscribeStream($"{_context.AreaName}-{_flowType.Name}-checkpoint", 0);

      return Task.Run(async () =>
      {
        do
        {
          await subscription.MoveNext();
          Observe(subscription.Current);
          await _onProjectionChanged();
        }
        while (true);
      });      
    }

    private void Observe(Message message)
    {
      var checkpointMetadata = JsonConvert.DeserializeObject<CheckpointMetadata>(System.Text.Encoding.UTF8.GetString(message.Header), new TimelinePositionConverter());
      UpdateProgress(message, checkpointMetadata);
    }

    //
    // Progress
    //

    private void UpdateProgress(Message message, CheckpointMetadata metadata)
    {
      var flowKey = message.Stream.Substring($"{_context.AreaName}-".Length, message.Stream.Length - $"{_context.AreaName}-".Length - "-checkpoint".Length);
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
  }
}