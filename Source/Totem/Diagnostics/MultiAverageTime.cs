using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average time to complete an operation
  /// </summary>
  public class MultiAverageTime : MultiCounter
  {
    internal MultiAverageTime(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.AverageTimer32);
      yield return NewBaseData(PerformanceCounterType.AverageBase);
    }

    public void Increment(long stopwatchTicks, Instance instance)
    {
      this[0, instance].IncrementBy(stopwatchTicks);
      this[1, instance].Increment();
    }

    public IDisposable IncrementAfter(Instance instance)
    {
      return TimeThenIncrement(instance, () => this[1, instance].Increment());
    }

    public IDisposable IncrementAfterBy(int amount, Instance instance)
    {
      return TimeThenIncrement(instance, () => this[1, instance].IncrementBy(amount));
    }

    IDisposable TimeThenIncrement(Instance instance, Action increment)
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