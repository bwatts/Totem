using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the average count of items per operation
  /// </summary>
  public class AverageCountM : MultiInstanceCounter
  {
    public AverageCountM(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.AverageCount64);
      yield return NewBaseData(PerformanceCounterType.AverageBase);
    }

    public void Add(string instance, long count)
    {
      this[0, instance].IncrementBy(count);
      this[1, instance].Increment();
    }
  }
}