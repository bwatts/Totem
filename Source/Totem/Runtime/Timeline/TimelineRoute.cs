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
		public TimelineRoute(FlowKey key, bool isFirst = false)
		{
			Key = key;
			IsFirst = isFirst;
		}

		public readonly FlowKey Key;
		public readonly bool IsFirst;

		public override string ToString() => Text.Of(Key).WriteIf(IsFirst, " (first)");
	}
}