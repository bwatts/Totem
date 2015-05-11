using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates a flow encountered an unhandled exception
	/// </summary>
	public class FlowFaulted : Event
	{
		public FlowFaulted(RuntimeTypeKey type, string fault)
		{
			Type = type;
			Fault = fault;
		}

		public readonly RuntimeTypeKey Type;
		public readonly string Fault;
	}
}