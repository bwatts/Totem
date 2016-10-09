using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a request on the timeline
	/// </summary>
	public sealed class RequestType : FlowType
	{
		internal RequestType(RuntimeTypeRef type, FlowConstructor constructor)
      : base(type, constructor, new Many<RuntimeTypeKey>())
		{}
	}
}