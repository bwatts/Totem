using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the rate of change of a 32-bit integer
  /// </summary>
  public class Rate : Counter
  {
    public Rate(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.RateOfCountsPerSecond32);
    }

    public void Increment()
    {
      this[0].Increment();
    }

    public void IncrementBy(int amount)
    {
      this[0].IncrementBy(amount);
    }
  }
}