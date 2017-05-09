using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures a fluctuating 32-bit integer
  /// </summary>
  public class MultiCount : MultiCounter
  {
    public MultiCount(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems32);
    }

    public void Increment(string instance)
    {
      this[0, instance].Increment();
    }

    public void Decrement(string instance)
    {
      this[0, instance].Decrement();
    }

    public void IncrementBy(int amount, string instance)
    {
      this[0, instance].IncrementBy(amount);
    }

    public void DecrementBy(int amount, string instance)
    {
      this[0, instance].IncrementBy(-amount);
    }

    public IDisposable IncrementDuring(string instance)
    {
      Increment(instance);

      return Disposal.Of(() => Decrement(instance));
    }

    public IDisposable DecrementDuring(string instance)
    {
      Decrement(instance);

      return Disposal.Of(() => Increment(instance));
    }

    public IDisposable IncrementDuringBy(int amount, string instance)
    {
      IncrementBy(amount, instance);

      return Disposal.Of(() => DecrementBy(amount, instance));
    }

    public IDisposable DecrementDuringBy(int amount, string instance)
    {
      DecrementBy(amount, instance);

      return Disposal.Of(() => IncrementBy(amount, instance));
    }
  }
}