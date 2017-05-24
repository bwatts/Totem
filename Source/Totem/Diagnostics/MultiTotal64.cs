using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures an increasing 64-bit integer
  /// </summary>
  public class MultiTotal64 : MultiCounter
  {
    internal MultiTotal64(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems64);
    }

    public void Increment(Instance instance)
    {
      this[0, instance].Increment();
    }

    public void IncrementBy(long amount, Instance instance)
    {
      this[0, instance].IncrementBy(amount);
    }
  }
}