using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the difference between samples of a 64-bit integer
  /// </summary>
  public class MultiDelta64 : MultiCounter
  {
    public MultiDelta64(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterDelta64);
    }

    public void Sample(long value, string instance)
    {
      this[0, instance].RawValue = value;
    }
  }
}