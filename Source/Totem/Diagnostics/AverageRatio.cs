using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average portion of successful operations to the total
  /// </summary>
  public class AverageRatio : Counter
  {
    public AverageRatio(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.SampleFraction);
      yield return NewData(PerformanceCounterType.SampleBase);
    }

    public void SampleTrue()
    {
      this[0].Increment();
      this[1].Increment();
    }

    public void SampleFalse()
    {
      this[1].Increment();
    }
  }
}