using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the difference between samples of a 64-bit integer
  /// </summary>
  public class Delta64 : Counter
  {
    internal Delta64(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterDelta64);
    }

    public void Sample(long value)
    {
      this[0].RawValue = value;
    }
  }
}