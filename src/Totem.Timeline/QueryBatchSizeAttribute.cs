using System;

namespace Totem.Timeline
{
  /// <summary>
  /// Indicates the batch size of the decorated <see cref="Query"/>
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class QueryBatchSizeAttribute : Attribute
  {
    public QueryBatchSizeAttribute(int batchSize)
    {
      BatchSize = batchSize;
    }

    public readonly int BatchSize;
  }
}