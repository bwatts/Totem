using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures a fluctuating 32-bit integer
  /// </summary>
  public class Count : Counter
  {
    public Count(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.NumberOfItems32);
    }

    public void Increment()
    {
      this[0].Increment();
    }

    public void Decrement()
    {
      this[0].Decrement();
    }

    public void IncrementBy(int amount)
    {
      this[0].IncrementBy(amount);
    }

    public void DecrementBy(int amount)
    {
      this[0].IncrementBy(-amount);
    }

    public IDisposable IncrementDuring()
    {
      Increment();

      return Disposal.Of(Decrement);
    }

    public IDisposable DecrementDuring()
    {
      Decrement();

      return Disposal.Of(Increment);
    }

    public IDisposable IncrementDuringBy(int amount)
    {
      IncrementBy(amount);

      return Disposal.Of(() => DecrementBy(amount));
    }

    public IDisposable DecrementDuringBy(int amount)
    {
      DecrementBy(amount);

      return Disposal.Of(() => IncrementBy(amount));
    }
  }
}