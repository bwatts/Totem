using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// The set of Given or When methods declared by a flow
	/// </summary>
	/// <typeparam name="T">The type of declared methods</typeparam>
	public sealed class FlowMethodSet<T> where T : FlowMethod
	{
		public FlowMethodSet(Many<T> methods, Many<T> scheduledMethods)
		{
			Methods = methods;
			ScheduledMethods = scheduledMethods;
		}

    public FlowMethodSet() : this(new Many<T>(), new Many<T>())
    {}

		public readonly Many<T> Methods;
		public readonly Many<T> ScheduledMethods;

		public Many<T> SelectMethods(TimelinePoint point)
		{
			return point.Scheduled ? ScheduledMethods : Methods;
		}
	}
}