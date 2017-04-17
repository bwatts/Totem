using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A multi-instance counter measuring an aspect of runtime performance
  /// </summary>
  public abstract class MultiInstanceCounter : Counter
  {
    readonly ConcurrentDictionary<string, PerformanceCounter> _counters = new ConcurrentDictionary<string, PerformanceCounter>();
    readonly List<string> _names = new List<string>();
    string _category;

    protected MultiInstanceCounter(string name, string description) : base(name, description)
    {}

    internal override bool CheckCardinality(CounterCategory category)
    {
      return category.IsMultiInstance;
    }

    internal override IEnumerable<CounterCreationData> GetCreationData(string category)
    {
      _category = category;

      var creationData = GetCreationData().ToList();

      foreach(var counterData in creationData)
      {
        _names.Add(counterData.CounterName);

        yield return counterData;
      }
    }

    protected abstract IEnumerable<CounterCreationData> GetCreationData();

    protected PerformanceCounter this[int creationDataIndex, string instance]
    {
      get
      {
        var name = _names[creationDataIndex];

        return _counters.GetOrAdd(name, _ => new PerformanceCounter(_category, name, instance, readOnly: false));
      }
    }
  }
}