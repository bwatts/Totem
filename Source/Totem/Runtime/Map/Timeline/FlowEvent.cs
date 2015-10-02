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
		}

		public readonly FlowType FlowType;
		public readonly EventType EventType;
		public readonly FlowMethodSet<FlowBefore> Before;
		public readonly FlowMethodSet<FlowWhen> When;

		public void CallBefore(Flow flow, TimelinePoint point)
		{
			foreach(var before in Before.SelectMethods(point))
			{
				flow.CallBefore(before, point);
			}
		}

		public async Task CallWhen(Flow flow, TimelinePoint point, IDependencySource dependencies)
		{
			CallBefore(flow, point);

			foreach(var when in When.SelectMethods(point))
			{
				await when.Call(flow, point.Event, dependencies);
			}
		}
	}
}