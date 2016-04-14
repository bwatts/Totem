using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
  /// A .NET type representing a persistent structure on the timeline
  /// </summary>
  public sealed class ViewType : QueryType
	{
		internal ViewType(RuntimeTypeRef type, FlowConstructor constructor) : base(type, constructor)
		{}
	}
}