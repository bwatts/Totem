using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StreamsDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// The projection installed to track the set of flows to resume
  /// </summary>
  public sealed class ResumeProjection : Notion, IResumeProjection
  {
    private readonly StreamsDbContext _context;
    private readonly ResumeProjectionState _state;

    public ResumeProjection(StreamsDbContext context)
    {
      _context = context;
      _state = new ResumeProjectionState();
    }

    public Task Synchronize()
    {
      var resumeAreaProjection = new ResumeAreaSubscriber(_context, _state, HandleProjectionChanged);
      resumeAreaProjection.Start();

      foreach (var flowType in _context.Area.FlowTypes)
      {
        var resumeProgressProjection = new ResumeProgressSubscriber(_context, _state, flowType, HandleProjectionChanged);
        resumeProgressProjection.Start();
      }

      Log.Debug("[timeline] Created projection {Name}", TimelineStreams.Resume);

      return Task.CompletedTask;
    }

    public async Task HandleProjectionChanged()
    {
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

      await _context.Client.DB().AppendStream($"{_context.AreaName}-{TimelineStreams.Resume}", messageInput);
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