using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// An event type observed within a flow
	/// </summary>
	public class FlowEvent
	{
		internal FlowEvent(FlowType flowType, EventType eventType, RouteMethod route, FlowMethodSet<WhenMethod> when)
		{
			FlowType = flowType;
			EventType = eventType;
			Route = route;
			HasRoute = route != null;
			When = when;

			if(!HasRoute)
			{
				SingleInstanceRoute = new FlowRoute(
					FlowKey.From(flowType),
					first: false,
					when: true,
					given: false,
					then: false);
			}
		}

		public readonly FlowType FlowType;
		public readonly EventType EventType;
		public readonly RouteMethod Route;
		public readonly bool HasRoute;
		public readonly FlowMethodSet<WhenMethod> When;
		protected readonly FlowRoute SingleInstanceRoute;

		public override string ToString() => $"{EventType} => {FlowType}";

		public IEnumerable<FlowRoute> RouteWhen(Event e, bool scheduled = false)
		{
			if(When.SelectMethods(scheduled).Count == 0)
			{
				yield break;
			}

			if(HasRoute)
			{
				foreach(var id in Route.Call(e))
				{
					yield return new FlowRoute(
						FlowKey.From(Route.FlowType, id),
						Route.First,
						when: true,
						given: false,
						then: false);
				}
			}
			else
			{
				yield return SingleInstanceRoute;
			}
		}

		public async Task CallWhen(Flow flow, FlowCall.When call)
		{
			foreach(var whenMethod in When.SelectMethods(call.Point.Scheduled))
			{
				await whenMethod.Call(flow, call.Point.Event, call.Dependencies);
			}
		}
	}
}