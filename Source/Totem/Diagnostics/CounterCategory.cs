using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// A container for a set of related performance counters
  /// </summary>
  public class CounterCategory
  {
    CounterCategory(string name, string description, bool isSingleInstance, bool isMultiInstance)
    {
      Name = name;
      Description = description;
      IsSingleInstance = isSingleInstance;
      IsMultiInstance = isMultiInstance;
    }

    public readonly string Name;
    public readonly string Description;
    public readonly bool IsSingleInstance;
    public readonly bool IsMultiInstance;

    public override string ToString() => Name;

    public static CounterCategory SingleInstance(string name, string description)
    {
      return new CounterCategory(name, description, true, false);
    }

    public static CounterCategory MultiInstance(string name, string description)
    {
      return new CounterCategory(name, description, false, true);
    }
  }
}