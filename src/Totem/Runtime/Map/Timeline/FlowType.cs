using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a flow on the timeline
	/// </summary>
	public sealed class FlowType : RuntimeType
	{
		internal FlowType(RuntimeTypeRef type, FlowMethodSet beforeMethods, FlowMethodSet whenMethods) : base(type)
		{
			BeforeMethods = beforeMethods;
			WhenMethods = whenMethods;
		}

		public readonly FlowMethodSet BeforeMethods;
		public readonly FlowMethodSet WhenMethods;

		public void CallWhen(Flow flow, Event e)
		{
			BeforeMethods.Call(flow, e);

			WhenMethods.Call(flow, e);
		}
	}
}