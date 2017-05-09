using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures part of a whole
  /// </summary>
  public class MultiPercentage : MultiCounter
  {
    public MultiPercentage(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.RawFraction);
      yield return NewData(PerformanceCounterType.RawBase);
    }

    public void Increment(string instance)
    {
      this[0, instance].Increment();
    }

    public void IncrementBy(int amount, string instance)
    {
      this[0, instance].IncrementBy(amount);
    }

    public void IncrementMax(string instance)
    {
      this[1, instance].Increment();
    }

    public void IncrementMax(int amount, string instance)
    {
      this[1, instance].IncrementBy(amount);
    }

    public void Decrement(string instance)
    {
      this[0, instance].Decrement();
    }

    public void DecrementBy(int amount, string instance)
    {
      this[0, instance].IncrementBy(-amount);
    }

    public void DecrementMax(string instance)
    {
      this[1, instance].Decrement();
    }

    public void DecrementMax(int amount, string instance)
    {
      this[1, instance].IncrementBy(-amount);
    }
  }
}