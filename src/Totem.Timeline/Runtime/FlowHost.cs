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
    readonly ITimelineDb _db;
    readonly IServiceProvider _services;

    internal FlowHost(ITimelineDb db, IServiceProvider services)
    {
      _db = db;
      _services = services;
    }

    internal async Task Resume(Many<FlowKey> routes)
    {
      foreach(var route in routes)
      {
        var flow = AddFlow(route);

        flow.ResumeWhenConnected();

        await ConnectFlow(flow);
      }
    }

    internal async Task OnNext(TimelinePoint point)
    {
      foreach(var route in point.Routes)
      {
        if(!Ignore(point, route))
        {
          await Enqueue(point, route);
        }
      }
    }

    //
    // Flows
    //

    IFlowScope AddFlow(FlowKey key)
    {
      var flow = key.Type.IsTopic ?
        new TopicScope(key, _db, _services) :
        new QueryScope(key, _db) as IFlowScope;

      _flowsByKey[key] = flow;

      RemoveWhenDone(flow);

      return flow;
    }

    async Task ConnectFlow(IFlowScope flow)
    {
      try
      {
        await flow.Connect(this);
      }
      catch(Exception error)
      {
        Log.Error(error, "Failed to connect flow {Flow}; treating as stopped", flow.Key);

        _stopped.Add(flow.Key);
      }
    }

    void RemoveWhenDone(IFlowScope flow) =>
      flow.LifetimeTask.ContinueWith(task =>
      {
        _flowsByKey.Remove(flow.Key);

        if(task.Status == TaskStatus.Faulted)
        {
          Log.Error(task.Exception, "[timeline] Flow lifetime ended with an error");

          _stopped.Add(flow.Key);
        }
        else
        {
          if(task.Result == FlowResult.Ignored)
          {
            _ignored.Add(flow.Key);
          }
        }
      });

    //
    // Points
    //

    bool Ignore(TimelinePoint point, FlowKey route)
    {
      if(!_ignored.Contains(route))
      {
        return false;
      }
      else if(route.Type.Observations.Get(point.Type).CanBeFirst)
      {
        _ignored.Remove(route);

        return false;
      }
      else
      {
        return true;
      }
    }

    async Task Enqueue(TimelinePoint point, FlowKey route)
    {
      if(_stopped.Contains(route))
      {
        return;
      }

      if(!_flowsByKey.TryGetValue(route, out var flow))
      {
        flow = AddFlow(route);

        await ConnectFlow(flow);
      }

      flow.Enqueue(point);
    }
  }
}