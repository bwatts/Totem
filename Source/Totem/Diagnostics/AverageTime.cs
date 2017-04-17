using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average time to complete an operation
  /// </summary>
  public class AverageTime : SingleInstanceCounter
  {
    public AverageTime(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.AverageTimer32);
      yield return NewBaseData(PerformanceCounterType.AverageBase);
    }

    public void Increment(long stopwatchTicks)
    {
      this[0].RawValue = stopwatchTicks;
      this[1].Increment();
    }

    public IDisposable IncrementAfter()
    {
      return TimeThenIncrement(() => this[1].Increment());
    }

    public IDisposable IncrementAfterBy(int amount)
    {
      return TimeThenIncrement(() => this[1].IncrementBy(amount));
    }

    IDisposable TimeThenIncrement(Action increment)
    {
      var start = Stopwatch.GetTimestamp();

      return Disposal.Of(() =>
      {
        this[0].RawValue = Stopwatch.GetTimestamp() - start;

        increment();
      });
    }
  }
}