using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures an increasing 32-bit integer
  /// </summary>
  public class Total32M : MultiInstanceCounter
  {
    public Total32M(string name, string description) : base(name, description)
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

    public void Increment(Id instance)
    {
      Increment(instance.ToString());
    }

    public void IncrementBy(int amount, Id instance)
    {
      IncrementBy(amount, instance.ToString());
    }
  }
}