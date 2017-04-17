using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A single-instance counter measuring an aspect of runtime performance
  /// </summary>
  public abstract class SingleInstanceCounter : Counter
  {
    readonly List<PerformanceCounter> _counters = new List<PerformanceCounter>();

    protected SingleInstanceCounter(string name, string description) : base(name, description)
    {}

    internal override bool CheckCardinality(CounterCategory category)
    {
      return category.IsSingleInstance;
    }

    internal sealed override IEnumerable<CounterCreationData> GetCreationData(string category)
    {
      _counters.Clear();

      foreach(var counterData in GetCreationData())
      {
        _counters.Add(new PerformanceCounter(category, counterData.CounterName, readOnly: false));

        yield return counterData;
      }
    }

    protected abstract IEnumerable<CounterCreationData> GetCreationData();

    protected PerformanceCounter this[int creationDataIndex] => _counters[creationDataIndex];
  }
}