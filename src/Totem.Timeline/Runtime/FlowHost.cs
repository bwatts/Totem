using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The set of topics and queries hosted by the timeline
  /// </summary>
  internal class FlowHost : Connection
  {
    readonly Dictionary<FlowKey, IFlowScope> _flowsByKey = new Dictionary<FlowKey, IFlowScope>();
    readonly HashSet<FlowKey> _ignored = new HashSet<FlowKey>();
    readonly HashSet<FlowKey> _stopped = new HashSet<FlowKey>();
    readonly IServiceProvider _services;
    readonly ITimelineDb _db;

    internal FlowHost(IServiceProvider services, ITimelineDb db)
    {
      _services = services;
      _db = db;
    }

    internal async Task Resume(Many<FlowKey> routes)
    {
      foreach(var route in routes)
      {
        if(!_ignored.Contains(route) && !_stopped.Contains(route))
        {
          await ConnectFlow(route);
        }
      }
    }

    internal async Task OnNext(TimelinePoint point)
    {
      foreach(var route in point.Routes)
      {
        if(_ignored.Contains(route))
        {
          if(route.Type.Observations.Get(point.Type).CanBeFirst)
          {
            _ignored.Remove(route);
          }
          else
          {
            continue;
          }
        }

        if(!_stopped.Contains(route))
        {
          (await GetOrConnectFlow(route)).Enqueue(point);
        }
      }
    }

    async Task<IFlowScope> ConnectFlow(FlowKey route)
    {
      var flow = CreateFlow(route);

      _flowsByKey[route] = flow;

      RemoveWhenDone(flow);

      await flow.Connect(State.CancellationToken);

      return flow;
    }

    async Task<IFlowScope> GetOrConnectFlow(FlowKey route)
    {
      if(!_flowsByKey.TryGetValue(route, out var flow))
      {
        flow = await ConnectFlow(route);

        _flowsByKey[route] = flow;
      }

      return flow;
    }

    IFlowScope CreateFlow(FlowKey key)
    {
      if(key.Type.IsTopic)
      {
        return new TopicScope(key, _db, _services);
      }
      else if(key.Type.IsQuery)
      {
        return new QueryScope(key, _db);
      }
      else
      {
        throw new Exception($"Expected topic or query type, received {key.Type}");
      }
    }

    void RemoveWhenDone(IFlowScope flow) =>
      flow.Task.ContinueWith(task => 
      {
        _flowsByKey.Remove(flow.Key);

        if(task.Status == TaskStatus.RanToCompletion)
        {
          switch(task.Result)
          {
            case FlowResult.Done:
              break;
            case FlowResult.Ignored:
              _ignored.Add(flow.Key);
              break;
            case FlowResult.Stopped:
              _stopped.Add(flow.Key);
              break;
            default:
              throw new NotSupportedException($"Unknown flow result: {task.Result}");
          }
        }
      });
  }
}