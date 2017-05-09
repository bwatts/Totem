using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average portion of successful operations to the total
  /// </summary>
  public class MultiAverageRatio : MultiCounter
  {
    public MultiAverageRatio(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.SampleFraction);
      yield return NewData(PerformanceCounterType.SampleBase);
    }

    public void SampleTrue(string instance)
    {
      this[0, instance].Increment();
      this[1, instance].Increment();
    }

    public void SampleFalse(string instance)
    {
      this[1, instance].Increment();
    }
  }
}