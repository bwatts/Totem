using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the ratio of active to inactive time of an operation
  /// </summary>
  public class MultiActivity : MultiCounter
  {
    internal MultiActivity(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.CounterTimer);
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