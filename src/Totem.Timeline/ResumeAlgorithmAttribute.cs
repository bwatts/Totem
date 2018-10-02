using System;

namespace Totem.Timeline
{
  /// <summary>
  /// Indicates the sizes of successive batches when reading resume points for the decorated <see cref="Topic"/> or <see cref="Query"/>
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class ResumeAlgorithmAttribute : Attribute
  {
    public ResumeAlgorithmAttribute(params int[] sizes)
    {
      Algorithm = new ResumeAlgorithm(sizes);
    }

    public readonly ResumeAlgorithm Algorithm;
  }
}