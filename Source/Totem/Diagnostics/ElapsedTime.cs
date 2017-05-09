using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the elapsed time from the start of an operation
  /// </summary>
  public class ElapsedTime : Counter
  {
    public ElapsedTime(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.ElapsedTime);
    }

    public void Start()
    {
      this[0].RawValue = Stopwatch.GetTimestamp();
    }
  }
}