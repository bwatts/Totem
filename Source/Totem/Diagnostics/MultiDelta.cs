using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the difference between samples of a 32-bit integer
  /// </summary>
  public class MultiDelta : MultiCounter
  {
    internal MultiDelta(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterDelta32);
    }

    public void Sample(int value, Instance instance)
    {
      this[0, instance].RawValue = value;
    }
  }
}