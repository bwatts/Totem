using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A monitored aspect of runtime performance with multiple instances
  /// </summary>
  public abstract class MultiCounter : CounterBase
  {
    readonly ConcurrentDictionary<Instance, PerformanceCounter> _counters = new ConcurrentDictionary<Instance, PerformanceCounter>();
    readonly List<string> _names = new List<string>();
    string _category;

    protected MultiCounter(string name, string description) : base(name, description)
    {}

    internal override bool CheckCardinality(CounterCategory category)
    {
      return category.IsMultiInstance;
    }

    internal override IEnumerable<CounterCreationData> GetCreationData(CounterCategory category)
    {
      _category = category.Name;

      foreach(var counterData in GetCreationData())
      {
        _names.Add(counterData.CounterName);

        yield return counterData;
      }
    }

    protected abstract IEnumerable<CounterCreationData> GetCreationData();

    protected PerformanceCounter this[int creationDataIndex, Instance instance]
    {
      get
      {
        // See CounterBase.cs for more on runtime instances

        var name = _names[creationDataIndex];

        return _counters.GetOrAdd(name, _ => new PerformanceCounter(
          _category,
          name,
          $"{RuntimePrefix}/{instance}",
          readOnly: false));
      }
    }

    public static MultiActivity Activity(string name, string description) => new MultiActivity(name, description);
    public static MultiActivityParallel ActivityParallel(string name, string description) => new MultiActivityParallel(name, description);
    public static MultiAverageCount AverageCount(string name, string description) => new MultiAverageCount(name, description);
    public static MultiAverageRatio AverageRatio(string name, string description) => new MultiAverageRatio(name, description);
    public static MultiAverageTime AverageTime(string name, string description) => new MultiAverageTime(name, description);
    public static MultiCount Count(string name, string description) => new MultiCount(name, description);
    public static MultiCount64 Count64(string name, string description) => new MultiCount64(name, description);
    public static MultiDelta Delta(string name, string description) => new MultiDelta(name, description);
    public static MultiDelta64 Delta64(string name, string description) => new MultiDelta64(name, description);
    public static MultiElapsedTime ElapsedTime(string name, string description) => new MultiElapsedTime(name, description);
    public static MultiInactivity Inactivity(string name, string description) => new MultiInactivity(name, description);
    public static MultiInactivityParallel InactivityParallel(string name, string description) => new MultiInactivityParallel(name, description);
    public static MultiPercentage Percentage(string name, string description) => new MultiPercentage(name, description);
    public static MultiRate Rate(string name, string description) => new MultiRate(name, description);
    public static MultiRate64 Rate64(string name, string description) => new MultiRate64(name, description);
    public static MultiTotal Total(string name, string description) => new MultiTotal(name, description);
    public static MultiTotal64 Total64(string name, string description) => new MultiTotal64(name, description);
  }
}