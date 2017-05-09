using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures an increasing 32-bit integer
  /// </summary>
  public class Total : Counter
  {
    public Total(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems32);
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