using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of active to inactive time of a parallel operation
  /// </summary>
  public class ActivityParallel : Counter
  {
    internal ActivityParallel(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterMultiTimer);
      yield return NewData(PerformanceCounterType.CounterMultiBase);
    }

    public void Increment(long stopwatchTicks)
    {
      this[0].IncrementBy(stopwatchTicks);
    }

    public IDisposable IncrementAfter()
    {
      var start = Stopwatch.GetTimestamp();

      return Disposal.Of(() =>
      {
        Increment(Stopwatch.GetTimestamp() - start);
      });
    }

    public void IncrementOps()
    {
      this[1].Increment();
    }

    public void IncrementOpsBy(int amount)
    {
      this[1].IncrementBy(amount);
    }

    public void SetOps(int count)
    {
      this[1].RawValue = count;
    }
  }
}