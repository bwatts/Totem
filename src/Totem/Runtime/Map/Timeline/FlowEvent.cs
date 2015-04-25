using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// An event observed by a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEvent
	{
		public FlowEvent(
			FlowType flowType,
			EventType eventType,
			IReadOnlyList<FlowEventBefore> beforeMethods,
			IReadOnlyList<FlowEventWhen> whenMethods,
			FlowEventWhen whenFirstMethod)
		{
			FlowType = flowType;
			EventType = eventType;
			BeforeMethods = beforeMethods;
			WhenMethods = whenMethods;
			WhenFirstMethod = whenFirstMethod;
			HasWhenFirstMethod = whenFirstMethod != null;

			EventType.FlowEvents.Register(this);
		}

		public readonly FlowType FlowType;
		public readonly EventType EventType;
		public readonly IReadOnlyList<FlowEventBefore> BeforeMethods;
		public readonly IReadOnlyList<FlowEventWhen> WhenMethods;
		public readonly FlowEventWhen WhenFirstMethod;
		public readonly bool HasWhenFirstMethod;

		public bool CanCallWhen(FlowEventContext context)
		{
			return EventType.IsInstance(context.Event);
		}

		public async Task CallWhenFirst(FlowEventContext context)
		{
			Expect.That(HasWhenFirstMethod).IsTrue("Flow event cannot be observed first");

			CallBefore(context);

			await WhenFirstMethod.Call(context);
		}

		public async Task CallWhen(FlowEventContext context)
		{
			CallBefore(context);

			foreach(var whenMethod in WhenMethods)
			{
				await whenMethod.Call(context);
			}
		}

		private void CallBefore(FlowEventContext context)
		{
			foreach(var beforeMethod in BeforeMethods)
			{
				beforeMethod.Call(context);
			}
		}
	}
}