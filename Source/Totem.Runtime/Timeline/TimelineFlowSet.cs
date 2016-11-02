using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of flows acting in a runtime
  /// </summary>
  internal sealed class TimelineFlowSet : Connection
	{
		readonly Dictionary<FlowKey, IFlowScope> _flowsByKey = new Dictionary<FlowKey, IFlowScope>();
		readonly TimelineScope _timeline;

    internal TimelineFlowSet(TimelineScope timeline)
    {
      _timeline = timeline;
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
          Log.Error("[timeline] Failed to push point {Point:l} to route {Route:l}", message.Point, route);

          if(!pushedRequestError)
          {
            pushedRequestError = true;

            _timeline.TryPushRequestError(message.Point.RequestId, error);
          }
        }
      }
    }

    void Push(TimelineMessage message, FlowRoute route)
    {
      IFlowScope flow;

      if(TryGetFlow(route, out flow) || TryReadFlow(route, out flow))
      {
        if(flow.Instance.Context.HasError)
        {
          _timeline.TryPushRequestError(message.Point.RequestId, new Exception($"Flow {flow.Key} is stopped"));
        }
        else
        {
          flow.Push(new FlowPoint(route, message.Point));
        }
      }
    }

    bool TryGetFlow(FlowRoute route, out IFlowScope flow)
    {
      return _flowsByKey.TryGetValue(route.Key, out flow);
    }

    bool TryReadFlow(FlowRoute route, out IFlowScope flow)
    {
      if(_timeline.TryReadFlow(route, out flow))
      {
        _flowsByKey[route.Key] = flow;

        var connection = flow.Connect(this);

        var capturedFlow = flow;

        flow.Task.ContinueWith(
          _ => FinishFlow(capturedFlow, connection),
          State.CancellationToken);
      }

      return flow != null;
    }

    void FinishFlow(IFlowScope flow, IDisposable connection)
    {
      _flowsByKey.Remove(flow.Key);

      connection.Dispose();

      if(flow.Task.IsFaulted)
      {
        _timeline.TryPushRequestError(flow.Point.RequestId, flow.Task.Exception);
      }
    }
	}
}