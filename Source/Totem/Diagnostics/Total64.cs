using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures an increasing 64-bit integer
  /// </summary>
  public class Total64 : SingleInstanceCounter
  {
    public Total64(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems64);
    }

    public void Increment()
    {
      this[0].Increment();
    }

    public void IncrementBy(long amount)
    {
      this[0].IncrementBy(amount);
    }
  }
}