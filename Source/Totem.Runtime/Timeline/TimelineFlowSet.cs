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
				IFlowScope flow;

				if(_flowsByKey.TryGetValue(route.Key, out flow))
				{
					flow.Push(message.Point);
				}
				else
				{
					if(_timeline.TryReadFlow(route, out flow))
					{
						_flowsByKey[route.Key] = flow;

						Connect(flow);

						flow.Push(message.Point);
					}
				}
			}
    }

		private void Connect(IFlowScope flow)
		{
			var connection = flow.Connect(this);

			flow.Task.ContinueWith(_ =>
			{
				_flowsByKey.Remove(flow.Key);

				connection.Dispose();
			},
			State.CancellationToken);
		}
	}
}