using   Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StreamsDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Totem.Timeline.Area;
using Totem.Timeline.Json;

namespace Totem.Timeline.StreamsDb.Projection
{
  public class ResumeProjection : IResumeProjection
  {
    private readonly StreamsDbContext _context;
    private readonly List<JsonConverter> _converters;

    private ResumeProjectionState _state;

    public ResumeProjection(StreamsDbContext context)
    {
      _context = context;
      _converters = new List<JsonConverter> { new TimelinePositionConverter(), new FlowKeyConverter(_context.Area) };
    }

    public async Task SynchronizeAsync()
    {
      var channel = Channel.CreateUnbounded<ResumeEvent>();

      var checkpoints = await GetCheckpoints();
      _state = new ResumeProjectionState(checkpoints);

      SubscribeToStream(channel.Writer, TimelineStreams.GetTimelineStream(_context.AreaName), ResumeEventStreamType.Timeline, checkpoints);

      foreach(var flowType in _context.Area.FlowTypes)
      {
        SubscribeToStream(channel.Writer, TimelineStreams.GetCheckpointStream(flowType, _context.AreaName), ResumeEventStreamType.Checkpoint, checkpoints);
      }
      
      StartProjection(channel);
    }

    private async Task<Dictionary<string, long>> GetCheckpoints()
    {
      var (message, found) = await _context.Client.DB().ReadLastMessageFromStream(TimelineStreams.GetResumeStream(_context.AreaName));

      var checkpoints = new Dictionary<string, long>()
      {
        { TimelineStreams.GetTimelineStream(_context.AreaName), 0 }
      };

      foreach (var flowType in _context.Area.FlowTypes)
      {
        var stream = TimelineStreams.GetCheckpointStream(flowType, _context.AreaName);
        checkpoints[stream] = 0;
      }

      if (found)
      {
        var resumeState = JsonConvert.DeserializeObject<ResumeState>(Encoding.UTF8.GetString(message.Value), _converters.ToArray());

        foreach (var checkpoint in resumeState.Checkpoints)
        {
          checkpoints[checkpoint.Key] = checkpoint.Value;
        }
      }

      return checkpoints;
    }

    private void SubscribeToStream(ChannelWriter<ResumeEvent> channelWriter, string stream, ResumeEventStreamType streamType, Dictionary<string, long> checkpoints)
    {
      var checkpoint = checkpoints[stream];
      var subscription = _context.Client.DB().SubscribeStream(stream, checkpoint + 1);

      Task.Run(async () =>
      {
        do
        {
          await subscription.MoveNext();

          var resumeEvent = new ResumeEvent
          {
            Message = subscription.Current,
            StreamType = streamType
          };

          await channelWriter.WriteAsync(resumeEvent);
        }
        while (true);
      });
    }

    private void StartProjection(Channel<ResumeEvent> channel)
    {
      Task.Run(async () =>
      {
        while (true)
        {
          var resumeEvent = await channel.Reader.ReadAsync();
          await Handle(resumeEvent);
        }
      });
    }

    private async Task Handle(ResumeEvent resumeEvent)
    {
      _state.Checkpoints[resumeEvent.Message.Stream] = resumeEvent.Message.Position;

      switch (resumeEvent.StreamType)
      {
        case ResumeEventStreamType.Timeline:
          var areaEventMetadata = JsonConvert.DeserializeObject<AreaEventMetadata>(Encoding.UTF8.GetString(resumeEvent.Message.Header), _converters.ToArray());
          await UpdateArea(resumeEvent.Message, areaEventMetadata);
          break;

        case ResumeEventStreamType.Checkpoint:
          var checkpointMetadata = JsonConvert.DeserializeObject<CheckpointMetadata>(Encoding.UTF8.GetString(resumeEvent.Message.Header), _converters.ToArray());
          UpdateProgress(resumeEvent.Message, checkpointMetadata);
          break;
      }

      var resumeState = GetResumeState();

      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resumeState, new JsonSerializerSettings
        {
          NullValueHandling = NullValueHandling.Include,
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        })),
        Type = nameof(ResumeState),
        Header = null
      };

      await _context.Client.DB().AppendStream(TimelineStreams.GetResumeStream(_context.AreaName), messageInput);
    }

    private async Task UpdateArea(Message message, AreaEventMetadata metadata)
    {
      await UpdateRoutes(message, metadata);
      await UpdateSchedule(message, metadata);
    }

    //
    // Routes
    //

    private async Task UpdateRoutes(Message message, AreaEventMetadata metadata)
    {
      var types = metadata.RouteTypes;

      foreach (var routeId in metadata.RouteIds)
      {
        var type = routeId.Type;
        var ids = routeId.Ids;

        foreach (var id in ids)
        {
          await UpdateMultiInstanceRoute(message, types[type], id);
        }

        types[type] = null;
      }

      foreach (var type in types)
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
        Type = $"singleInstanceRoute:{message.Type}",
        Value = message.Value,
        Header = message.Header
      };

      await _context.Client.DB().AppendStream(type.GetRoutesStream(_context.AreaName), messageInput);

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

      instance.Latest = _state.Checkpoints[message.Stream];
    }

    private async Task UpdateMultiInstanceRoute(Message message, AreaTypeName type, Id id)
    {
      var messageInput = new MessageInput
      {
        ID = Guid.NewGuid().ToString(),
        Type = $"multiInstanceRoute:{message.Type}",
        Value = message.Value,
        Header = message.Header
      };
      
      await _context.Client.DB().AppendStream(type.GetRoutesStream(_context.AreaName, id), messageInput);

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

      instance.Latest = _state.Checkpoints[message.Stream];
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

      await _context.Client.DB().AppendStream(TimelineStreams.GetScheduleStream(_context.AreaName), messageInput);

      _state.Schedule[_state.Checkpoints[message.Stream]] = null;
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

    //
    // Resume state
    //

    private ResumeState GetResumeState()
    {
      var routes = BuildRoutes();
      var schedule = BuildSchedule();

      return new ResumeState
      {
        Checkpoints = _state.Checkpoints,
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

  public class ResumeEvent
  {
    public Message Message { get; set; }
    public ResumeEventStreamType StreamType { get; set; }
  }

  public enum ResumeEventStreamType
  {
    Timeline,
    Checkpoint
  }
}