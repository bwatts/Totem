using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures a fluctuating 64-bit integer
  /// </summary>
  public class Count64M : MultiInstanceCounter
  {
    public Count64M(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems64);
    }

    public void Increment(string instance)
    {
      this[0, instance].Increment();
    }

    public void Decrement(string instance)
    {
      this[0, instance].Decrement();
    }

    public void IncrementBy(long amount, string instance)
    {
      this[0, instance].IncrementBy(amount);
    }

    public void DecrementBy(long amount, string instance)
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

    public IDisposable IncrementDuringBy(long amount, string instance)
    {
      IncrementBy(amount, instance);

      return Disposal.Of(() => DecrementBy(amount, instance));
    }

    public IDisposable DecrementDuringBy(long amount, string instance)
    {
      DecrementBy(amount, instance);

      return Disposal.Of(() => IncrementBy(amount, instance));
    }

    public void Increment(Id instance)
    {
      Increment(instance.ToString());
    }

    public void Decrement(Id instance)
    {
      Decrement(instance.ToString());
    }

    public void IncrementBy(long amount, Id instance)
    {
      IncrementBy(amount, instance.ToString());
    }

    public void DecrementBy(long amount, Id instance)
    {
      DecrementBy(amount, instance.ToString());
    }

    public IDisposable IncrementDuring(Id instance)
    {
      return IncrementDuring(instance.ToString());
    }

    public IDisposable DecrementDuring(Id instance)
    {
      return DecrementDuring(instance.ToString());
    }

    public IDisposable IncrementDuringBy(long amount, Id instance)
    {
      return IncrementDuringBy(amount, instance.ToString());
    }

    public IDisposable DecrementDuringBy(long amount, Id instance)
    {
      return DecrementDuringBy(amount, instance.ToString());
    }
  }
}