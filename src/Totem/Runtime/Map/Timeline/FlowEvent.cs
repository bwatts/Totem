using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// An event observed by a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEvent : Notion
	{
		public FlowEvent(
			FlowType flowType,
			EventType eventType,
			FlowMethodSet<FlowBefore> before,
			FlowMethodSet<FlowWhen> when)
		{
			FlowType = flowType;
			EventType = eventType;
			Before = before;
			When = when;

			EventType.FlowEvents.Register(this);
		}

		public readonly FlowType FlowType;
		public readonly EventType EventType;
		public readonly FlowMethodSet<FlowBefore> Before;
		public readonly FlowMethodSet<FlowWhen> When;

		public void TryCallBefore(Flow flow, TimelinePoint point)
		{
			if(EventType.CanAssign(point))
			{
				foreach(var before in Before.SelectMethods(point))
				{
					before.Call(flow, point.Event);
				}
			}
		}

		public async Task TryCallWhen(Flow flow, TimelinePoint point, IDependencySource dependencies)
		{
			if(EventType.CanAssign(point))
			{
				foreach(var before in Before.SelectMethods(point))
				{
					before.Call(flow, point.Event);
				}

				foreach(var when in When.SelectMethods(point))
				{
					await when.Call(flow, point.Event, dependencies);
				}
			}
		}
	}
}