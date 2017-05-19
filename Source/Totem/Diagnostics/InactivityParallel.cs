using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of inactive to active time of a parallel operation
  /// </summary>
  public class InactivityParallel : Counter
  {
    internal InactivityParallel(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterMultiTimerInverse);
      yield return NewData(PerformanceCounterType.CounterMultiBase);
    }

    public void Increment(long stopwatchTicks)
    {
      this[0].IncrementBy(stopwatchTicks);
    }

    public void SetOps(int count)
    {
      this[1].RawValue = count;
    }
  }
}