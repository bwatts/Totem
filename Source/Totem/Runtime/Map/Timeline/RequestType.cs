using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
  /// A .NET type representing a request on the timeline
  /// </summary>
  public class RequestType : FlowType
	{
		internal RequestType(
      RuntimeTypeRef type,
      FlowConstructor constructor,
      RequestStartMethod startMethod)
      : base(type, constructor, new Many<RuntimeTypeKey>())
		{
      StartMethod = startMethod;
    }

    public readonly RequestStartMethod StartMethod;
  }
}