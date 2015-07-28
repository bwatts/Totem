using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing an event on the timeline
	/// </summary>
	public sealed class EventType : RuntimeType
	{
		public EventType(RuntimeTypeRef type) : base(type)
		{}

		public bool CanAssign(TimelinePoint point)
		{
			return CanAssign(point.EventType);
		}
	}
}