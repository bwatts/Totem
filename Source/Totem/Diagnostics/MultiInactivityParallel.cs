using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of inactive to active time of a parallel operation
  /// </summary>
  public class MultiInactivityParallel : MultiCounter
  {
    internal MultiInactivityParallel(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterMultiTimerInverse);
      yield return NewData(PerformanceCounterType.CounterMultiBase);
    }

    public void Increment(long stopwatchTicks, Instance instance)
    {
      this[0, instance].IncrementBy(stopwatchTicks);
    }

    public IDisposable IncrementAfter(Instance instance)
    {
      var start = Stopwatch.GetTimestamp();

      return Disposal.Of(() =>
      {
        Increment(Stopwatch.GetTimestamp() - start, instance);
      });
    }

    public void IncrementOps(Instance instance)
    {
      this[1, instance].Increment();
    }

    public void IncrementOpsBy(int amount, Instance instance)
    {
      this[1, instance].IncrementBy(amount);
    }

    public void SetOps(int count, Instance instance)
    {
      this[1, instance].RawValue = count;
    }
  }
}