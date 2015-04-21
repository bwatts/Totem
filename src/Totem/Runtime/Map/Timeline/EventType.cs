using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing an event on the timeline
	/// </summary>
	public sealed class EventType : RuntimeType
	{
		internal EventType(RuntimeTypeRef type) : base(type)
		{
			
		}
	}
}