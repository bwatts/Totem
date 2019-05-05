using System;

namespace Totem.Timeline
{
  /// <summary>
  /// The sizes of successive batches when reading resume points
  /// </summary>
  public class ResumeAlgorithm
  {
    readonly int[] _sizes;

    public ResumeAlgorithm(params int[] sizes)
    {
      _sizes = sizes.Length > 0 ? sizes : new[] { 10, 50, 100, 200 };
    }

    public int GetNextBatchSize(int batchIndex) =>
      _sizes[Math.Max(batchIndex, _sizes.Length - 1)];
  }
}