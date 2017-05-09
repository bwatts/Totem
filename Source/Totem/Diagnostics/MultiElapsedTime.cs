﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Measures the elapsed time from the start of an operation
  /// </summary>
  public class MultiElapsedTime : MultiCounter
  {
    internal MultiElapsedTime(string name, string description) : base(name, description)
    {}

    protected override IEnumerable<CounterCreationData> GetCreationData()
    {
      yield return NewData(PerformanceCounterType.ElapsedTime);
    }

    public void Start(Instance instance)
    {
      this[0, instance].RawValue = Stopwatch.GetTimestamp();
    }
  }
}