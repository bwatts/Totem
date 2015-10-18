using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a topic on the timeline
	/// </summary>
	public sealed class TopicType : FlowType
	{
		internal TopicType(RuntimeTypeRef type, FlowConstructor constructor) : base(type, constructor)
		{}
	}
}