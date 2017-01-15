using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of flows acting in a runtime
  /// </summary>
  internal sealed class TimelineFlowSet : Connection
	{
		readonly ConcurrentDictionary<FlowKey, IFlowScope> _flowsByKey = new ConcurrentDictionary<FlowKey, IFlowScope>();
		readonly TimelineScope _timeline;
    Dictionary<FlowKey, TimelinePosition> _resumeCheckpoints;

    internal TimelineFlowSet(TimelineScope timeline)
    {
      _timeline = timeline;
    }

    internal void ResumeWith(ResumeInfo info)
    {
      _resumeCheckpoints = info.Flows.ToDictionary(
        flow => flow.Key,
        flow => flow.Checkpoint);
    }

    internal void Push(TimelineMessage message)
    {
      var pushedRequestError = false;

			foreach(var route in message.Routes)
			{
        try
        {
          Push(message, route);
        }
        catch(Exception error)
        {
          Log.Error(error, "[timeline] [{Flow:l}] Failed to push point {Point:l}", route.Key, message.Point);

          if(!pushedRequestError)
          {
            pushedRequestError = true;

            _timeline.TryPushRequestError(message.Point, error);
          }
        }
      }
    }

    void Push(TimelineMessage message, FlowRoute route)
    {
      var flow = ReadFlow(route);

      if(flow.Task.IsFaulted)
      {
        _timeline.TryPushRequestError(message.Point, flow.Task.Exception);
      }
      else
      {
        flow.Push(new FlowPoint(route, message.Point));
      }
    }

    IFlowScope ReadFlow(FlowRoute route)
    {
      IFlowScope flow;

      if(!_flowsByKey.TryGetValue(route.Key, out flow))
      {
        flow = CreateFlow(route);

        _flowsByKey[route.Key] = flow;
      }

      return flow;
    }

    IFlowScope CreateFlow(FlowRoute route)
    {
      var flow = _timeline.OpenFlowScope(route);

      ConnectFlow(flow);
      
      TryResume(flow);

      return flow;
    }

    void ConnectFlow(IFlowScope flow)
    {
      var connection = flow.Connect(this);

      flow.Task.ContinueWith(
        _ => FinishFlow(flow, connection),
        State.CancellationToken);
    }

    void TryResume(IFlowScope flow)
    {
      TimelinePosition checkpoint;

      if(TryReadResumeCheckpoint(flow, out checkpoint))
      {
        flow.ResumeTo(checkpoint);

        RemoveResumeCheckpoint(flow);
      }
    }

    bool TryReadResumeCheckpoint(IFlowScope flow, out TimelinePosition checkpoint)
    {
      if(_resumeCheckpoints == null)
      {
        checkpoint = TimelinePosition.None;

        return false;
      }

      return _resumeCheckpoints.TryGetValue(flow.Key, out checkpoint);
    }

    void RemoveResumeCheckpoint(IFlowScope flow)
    {
      _resumeCheckpoints.Remove(flow.Key);

      if(_resumeCheckpoints.Count == 0)
      {
        _resumeCheckpoints = null;
      }
    }

    void FinishFlow(IFlowScope flow, IDisposable connection)
    {
      IFlowScope removed;

      _flowsByKey.TryRemove(flow.Key, out removed);

      connection.Dispose();

      if(flow.Task.IsFaulted)
      {
        _timeline.TryPushRequestError(flow.ErrorPoint, flow.Task.Exception);
      }
    }
	}
}