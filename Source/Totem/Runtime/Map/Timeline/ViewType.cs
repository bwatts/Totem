using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
  /// <summary>
  /// A .NET type representing a persistent structure on the timeline
  /// </summary>
  public sealed class ViewType : FlowType
	{
		internal ViewType(RuntimeTypeRef type, FlowConstructor constructor, Many<RuntimeTypeKey> priorKeys, int batchSize)
      : base(type, constructor, priorKeys)
		{
      BatchSize = batchSize;
    }

    public readonly int BatchSize;

    public const int DefaultBatchSize = 200;
	}
}