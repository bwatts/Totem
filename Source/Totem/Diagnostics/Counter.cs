using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A counter measuring an aspect of runtime performance
  /// </summary>
  public abstract class Counter
  {
    protected Counter(string name, string description)
    {
      Name = name;
      Description = description;
    }

    public readonly string Name;
    public readonly string Description;

    public override string ToString() => Name;

    internal abstract bool CheckCardinality(CounterCategory category);

    internal abstract IEnumerable<CounterCreationData> GetCreationData(string category);

    protected CounterCreationData NewData(PerformanceCounterType type)
    {
      return new CounterCreationData(Name, Description, type);
    }

    protected CounterCreationData NewBaseData(PerformanceCounterType type, string suffix = " (base)")
    {
      return new CounterCreationData(Name + suffix, Description + suffix, type);
    }

    //
    // Factory
    //

    public static AverageCount AverageCount(string name, string description)
    {
      return new AverageCount(name, description);
    }

    public static AverageTime AverageTime(string name, string description)
    {
      return new AverageTime(name, description);
    }

    public static Count32 Count32(string name, string description)
    {
      return new Count32(name, description);
    }

    public static Count64 Count64(string name, string description)
    {
      return new Count64(name, description);
    }

    public static ElapsedTime ElapsedTime(string name, string description)
    {
      return new ElapsedTime(name, description);
    }

    public static Total32 Total32(string name, string description)
    {
      return new Total32(name, description);
    }

    public static Total64 Total64(string name, string description)
    {
      return new Total64(name, description);
    }

    //
    // Factory (multi-instance)
    //

    public static AverageCountM AverageCountM(string name, string description)
    {
      return new AverageCountM(name, description);
    }

    public static AverageTimeM AverageTimeM(string name, string description)
    {
      return new AverageTimeM(name, description);
    }

    public static Count32M Count32M(string name, string description)
    {
      return new Count32M(name, description);
    }

    public static ElapsedTimeM ElapsedTimeM(string name, string description)
    {
      return new ElapsedTimeM(name, description);
    }

    public static Count64M Count64M(string name, string description)
    {
      return new Count64M(name, description);
    }

    public static Total32M Total32M(string name, string description)
    {
      return new Total32M(name, description);
    }

    public static Total64M Total64M(string name, string description)
    {
      return new Total64M(name, description);
    }
  }
}