using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures an increasing 32-bit integer
  /// </summary>
  public class MultiTotal : MultiCounter
  {
    public MultiTotal(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems32);
    }

    public void Increment(string instance)
    {
      this[0, instance].Increment();
    }

    public void IncrementBy(int amount, string instance)
    {
      this[0, instance].IncrementBy(amount);
    }
  }
}