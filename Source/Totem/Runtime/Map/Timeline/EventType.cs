using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing an event on the timeline
	/// </summary>
	public sealed class EventType : DurableType
	{
		public EventType(RuntimeTypeRef type) : base(type)
		{}

		public readonly Many<FlowEvent> Flows = new Many<FlowEvent>();

		internal void RegisterFlow(FlowEvent flow)
		{
			Flows.Write.Add(flow);
		}

		public IEnumerable<FlowRoute> RouteWhen(Event e, bool scheduled = false)
		{
			return Flows
				.Where(flow => !flow.FlowType.IsRequest)
				.SelectMany(flow => flow.RouteWhen(e, scheduled));
		}

		public IEnumerable<FlowRoute> RouteGiven(Event e, bool scheduled = false)
		{
			return Flows
				.Where(flow => flow.FlowType.IsTopic)
				.Cast<TopicEvent>()
				.SelectMany(topic => topic.RouteGiven(e, scheduled));
		}
	}
}