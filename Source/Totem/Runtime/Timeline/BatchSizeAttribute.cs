using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates the batch size of the decorated <see cref="View"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class BatchSizeAttribute : Attribute
	{
    public BatchSizeAttribute(int batchSize)
    {
      BatchSize = batchSize;
    }

    public readonly int BatchSize;
	}
}