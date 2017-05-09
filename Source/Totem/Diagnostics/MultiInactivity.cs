using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of inactive to active time of an operation
  /// </summary>
  public class MultiInactivity : MultiCounter
  {
    internal MultiInactivity(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterTimerInverse);
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
  }
}