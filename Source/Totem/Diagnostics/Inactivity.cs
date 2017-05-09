using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of inactive to active time of an operation
  /// </summary>
  public class Inactivity : Counter
  {
    internal Inactivity(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterTimerInverse);
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
  }
}