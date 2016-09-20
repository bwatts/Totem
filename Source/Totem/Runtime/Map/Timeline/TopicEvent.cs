using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// An event observed within a <see cref="Topic"/>
	/// </summary>
	public sealed class TopicEvent : FlowEvent
	{
		internal TopicEvent(
			FlowType flowType,
			EventType eventType,
			RouteMethod route,
			FlowMethodSet<WhenMethod> when,
			FlowMethodSet<GivenMethod> given)
			: base(flowType, eventType, route, when)
		{
			Given = given;
		}

		public readonly FlowMethodSet<GivenMethod> Given;

		public IEnumerable<FlowRoute> RouteGiven(Event e, bool scheduled = false, FlowKey topicKey = null)
		{
			if(Given.SelectMethods(scheduled).Count == 0)
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
						when: false,
						given: true,
						then: false);
				}
			}
			else
			{
				yield return SingleInstanceRoute;
			}
		}

		public void CallGiven(Topic topic, FlowCall.Given call)
		{
			foreach(var givenMethod in Given.SelectMethods(call.Point.Scheduled))
			{
				givenMethod.Call(topic, call.Point.Event);
			}
		}
	}
}