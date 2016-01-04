using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A method observing an event in a <see cref="Flow"/>
	/// </summary>
	public abstract class FlowMethod
	{
		protected FlowMethod(MethodInfo info, EventType eventType)
		{
			Info = info;
			EventType = eventType;
		}

		public readonly MethodInfo Info;
		public readonly EventType EventType;
	}
}