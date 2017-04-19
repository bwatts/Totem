using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average time to complete an operation
  /// </summary>
  public class AverageTimeM : MultiInstanceCounter
  {
    public AverageTimeM(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.AverageTimer32);
      yield return NewBaseData(PerformanceCounterType.AverageBase);
    }

    public void Increment(long stopwatchTicks, string instance)
    {
      this[0, instance].IncrementBy(stopwatchTicks);
      this[1, instance].Increment();
    }

    public IDisposable IncrementAfter(string instance)
    {
      return TimeThenIncrement(instance, () => this[1, instance].Increment());
    }

    public IDisposable IncrementAfterBy(int amount, string instance)
    {
      return TimeThenIncrement(instance, () => this[1, instance].IncrementBy(amount));
    }

    public void Increment(long stopwatchTicks, Id instance)
    {
      Increment(stopwatchTicks, instance.ToString());
    }

    public IDisposable IncrementAfter(Id instance)
    {
      return IncrementAfter(instance.ToString());
    }

    public IDisposable IncrementAfterBy(int amount, Id instance)
    {
      return IncrementAfterBy(amount, instance.ToString());
    }

    IDisposable TimeThenIncrement(string instance, Action increment)
    {
      var start = Stopwatch.GetTimestamp();

      return Disposal.Of(() =>
      {
        this[0, instance].IncrementBy(Stopwatch.GetTimestamp() - start);

        increment();
      });
    }
  }
}