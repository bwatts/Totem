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
			IReadOnlyList<FlowEventBefore> beforeMethods,
			IReadOnlyList<FlowEventWhen> whenMethods)
		{
			FlowType = flowType;
			EventType = eventType;
			BeforeMethods = beforeMethods;
			WhenMethods = whenMethods;

			EventType.FlowEvents.Register(this);
		}

		public readonly FlowType FlowType;
		public readonly EventType EventType;
		public readonly IReadOnlyList<FlowEventBefore> BeforeMethods;
		public readonly IReadOnlyList<FlowEventWhen> WhenMethods;

		public bool CanCall(Event e)
		{
			return EventType.IsInstance(e);
		}

		public void CallBefore(Flow flow, Event e)
		{
			foreach(var beforeMethod in BeforeMethods)
			{
				beforeMethod.Call(flow, e);
			}
		}

		public async Task CallWhen(Flow flow, Event e, IDependencySource dependencies)
		{
			CallBefore(flow, e);

			foreach(var whenMethod in WhenMethods)
			{
				await whenMethod.Call(flow, e, dependencies);
			}
		}
	}
}