using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A monitored aspect of runtime performance with a single instance
  /// </summary>
  public abstract class Counter : CounterBase
  {
    readonly List<Lazy<PerformanceCounter>> _systemCounters = new List<Lazy<PerformanceCounter>>();

    protected Counter(string name, string description) : base(name, description)
    {}

    internal override bool CheckCardinality(CounterCategory category)
    {
      return category.IsSingleInstance;
    }

    internal sealed override IEnumerable<CounterCreationData> GetCreationData(CounterCategory category)
    {
      _systemCounters.Clear();

      foreach(var counterData in GetCreationData())
      {
        AddSystemCounter(category.Name, counterData.CounterName);

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

    public static Activity Activity(string name, string description) => new Activity(name, description);
    public static ActivityParallel ActivityParallel(string name, string description) => new ActivityParallel(name, description);
    public static AverageCount AverageCount(string name, string description) => new AverageCount(name, description);
    public static AverageRatio Ratio(string name, string description) => new AverageRatio(name, description);
    public static AverageTime AverageTime(string name, string description) => new AverageTime(name, description);
    public static Count Count(string name, string description) => new Count(name, description);
    public static Count64 Count64(string name, string description) => new Count64(name, description);
    public static Delta Delta(string name, string description) => new Delta(name, description);
    public static Delta64 Delta64(string name, string description) => new Delta64(name, description);
    public static ElapsedTime ElapsedTime(string name, string description) => new ElapsedTime(name, description);
    public static Inactivity Inactivity(string name, string description) => new Inactivity(name, description);
    public static InactivityParallel InactivityParallel(string name, string description) => new InactivityParallel(name, description);
    public static Percentage Percentage(string name, string description) => new Percentage(name, description);
    public static Rate Rate(string name, string description) => new Rate(name, description);
    public static Rate64 Rate64(string name, string description) => new Rate64(name, description);
    public static Total Total(string name, string description) => new Total(name, description);
    public static Total64 Total64(string name, string description) => new Total64(name, description);
  }
}