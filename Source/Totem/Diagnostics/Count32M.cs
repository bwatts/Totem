using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures a fluctuating 32-bit integer
  /// </summary>
  public class Count32M : MultiInstanceCounter
  {
    public Count32M(string name, string description) : base(name, description)
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

    public void Increment(Id instance)
    {
      Increment(instance.ToString());
    }

    public void Decrement(Id instance)
    {
      Decrement(instance.ToString());
    }

    public void IncrementBy(int amount, Id instance)
    {
      IncrementBy(amount, instance.ToString());
    }

    public void DecrementBy(int amount, Id instance)
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

    public IDisposable IncrementDuringBy(int amount, Id instance)
    {
      return IncrementDuringBy(amount, instance.ToString());
    }

    public IDisposable DecrementDuringBy(int amount, Id instance)
    {
      return DecrementDuringBy(amount, instance.ToString());
    }
  }
}