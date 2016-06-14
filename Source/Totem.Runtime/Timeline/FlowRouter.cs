using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Routes timeline points to a type of flow
  /// </summary>
  internal sealed class FlowRouter : Connection
  {
    private readonly Dictionary<Id, IFlowScope> _scopesById = new Dictionary<Id, IFlowScope>();
    private readonly FlowType _flow;
    private readonly ITimelineScope _timeline;

    internal FlowRouter(FlowType flow, ITimelineScope timeline)
    {
      _flow = flow;
      _timeline = timeline;
    }

    public override Text ToText()
    {
      return $"{_flow} ({Text.Count(_scopesById.Count, "scope")})";
    }

    internal void Push(TimelinePoint point)
    {
			foreach(var route in _flow.CallRoute(point))
			{
				IFlowScope scope;

				if(TryGetScope(route, out scope) || TryOpenScope(route, out scope))
				{
					scope.Push(point);
				}
			}
    }

		private bool TryGetScope(TimelineRoute route, out IFlowScope scope)
		{
			return _scopesById.TryGetValue(route.Id, out scope);
    }

		private bool TryOpenScope(TimelineRoute route, out IFlowScope scope)
		{
			scope = null;

			if(_timeline.TryOpenFlowScope(_flow, route, out scope))
			{
				var connection = scope.Connect(this);

				_scopesById[route.Id] = scope;

				RemoveWhenDone(scope, connection);
			}

			return scope != null;
		}

		private void RemoveWhenDone(IFlowScope scope, IDisposable connection)
    {
      scope.Task.ContinueWith(_ =>
      {
        _scopesById.Remove(scope.Key.Id);

        connection.Dispose();
      },
      State.CancellationToken);
    }
  }
}