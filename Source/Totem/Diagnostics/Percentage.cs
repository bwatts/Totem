using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures part of a whole
  /// </summary>
  public class Percentage : Counter
  {
    public Percentage(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.RawFraction);
      yield return NewData(PerformanceCounterType.RawBase);
    }

    public void Increment()
    {
      this[0].Increment();
    }

    public void IncrementBy(int amount)
    {
      this[0].IncrementBy(amount);
    }

    public void IncrementMax()
    {
      this[1].Increment();
    }

    public void IncrementMax(int amount)
    {
      this[1].IncrementBy(amount);
    }

    public void Decrement()
    {
      this[0].Decrement();
    }

    public void DecrementBy(int amount)
    {
      this[0].IncrementBy(-amount);
    }

    public void DecrementMax()
    {
      this[1].Decrement();
    }

    public void DecrementMax(int amount)
    {
      this[1].IncrementBy(-amount);
    }
  }
}