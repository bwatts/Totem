using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the rate of change of a 64-bit integer
  /// </summary>
  public class MultiRate64 : MultiCounter
  {
    internal MultiRate64(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.RateOfCountsPerSecond64);
    }

    public void Increment(Instance instance)
    {
      this[0, instance].Increment();
    }

    public void IncrementBy(int amount, Instance instance)
    {
      this[0, instance].IncrementBy(amount);
    }
  }
}