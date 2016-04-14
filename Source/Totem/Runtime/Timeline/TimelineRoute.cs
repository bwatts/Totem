using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Routes a timeline point to a flow instance
	/// </summary>
	public class TimelineRoute
	{
		public TimelineRoute(Id id, bool isFirst = false)
		{
			Id = id;
			IsFirst = isFirst;
		}

		public readonly Id Id;
		public readonly bool IsFirst;

		public override string ToString() => Text.Of(Id).WriteIf(IsFirst, " (first)");

		public static readonly TimelineRoute SingleInstance = new TimelineRoute(Id.Unassigned);
	}
}