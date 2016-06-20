using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The set of flows acting in a runtime
  /// </summary>
  internal sealed class TimelineFlowSet : Connection
	{
		private readonly Dictionary<FlowKey, IFlowScope> _flowsByKey = new Dictionary<FlowKey, IFlowScope>();

		private readonly TimelineScope _timeline;

    internal TimelineFlowSet(TimelineScope timeline)
    {
      _timeline = timeline;
    }

    internal void Push(TimelineMessage message)
    {
			foreach(var route in message.Routes)
			{
				Push(message.Point, route);
			}
    }

		private void Push(TimelinePoint point, TimelineRoute route)
		{
			IFlowScope flow;

			if(!TryGetFlow(route, out flow))
			{
				flow = ReadFlow(route);
			}

			flow.Push(point);
		}

		private bool TryGetFlow(TimelineRoute route, out IFlowScope flow)
		{
			return _flowsByKey.TryGetValue(route.Key, out flow);
		}

		private IFlowScope ReadFlow(TimelineRoute route)
		{
			var flow = _timeline.ReadFlow(route);

			var connection = flow.Connect(this);

			_flowsByKey[route.Key] = flow;

			RemoveWhenDone(flow, connection);

			return flow;
		}

		private void RemoveWhenDone(IFlowScope flow, IDisposable connection)
		{
			flow.Task.ContinueWith(_ =>
			{
				_flowsByKey.Remove(flow.Key);

				connection.Dispose();
			},
			State.CancellationToken);
		}
	}
}