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
    readonly List<Lazy<PerformanceCounter>> _systemCounters = new List<Lazy<PerformanceCounter>>();

    protected SingleInstanceCounter(string name, string description) : base(name, description)
    {}

    internal override bool CheckCardinality(CounterCategory category)
    {
      return category.IsSingleInstance;
    }

    internal sealed override IEnumerable<CounterCreationData> GetCreationData(string category)
    {
      _systemCounters.Clear();

      foreach(var counterData in GetCreationData())
      {
        AddSystemCounter(category, counterData.CounterName);

        yield return counterData;
      }
    }

    protected abstract IEnumerable<CounterCreationData> GetCreationData();

    protected PerformanceCounter this[int creationDataIndex] => _systemCounters[creationDataIndex].Value;

    void AddSystemCounter(string category, string name)
    {
      _systemCounters.Add(new Lazy<PerformanceCounter>(() =>
        new PerformanceCounter(category, name, RuntimePrefix, readOnly: false)));
    }
  }
}