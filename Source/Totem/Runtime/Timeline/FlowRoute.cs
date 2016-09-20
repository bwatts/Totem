using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Routes a timeline point to an instance of a flow
	/// </summary>
	public class FlowRoute
	{
		public FlowRoute(FlowKey key, bool first, bool when, bool given, bool then)
		{
			Key = key;
			First = first;
			When = when;
			Given = given;
			Then = then;
		}

		public readonly FlowKey Key;
		public readonly bool First;
		public readonly bool When;
		public readonly bool Given;
		public readonly bool Then;

		public override string ToString() => Key.ToString();
	}
}