using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Indicates counters in the decorated class belong to the specified category
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public abstract class CounterCategoryAttribute : Attribute
  {
    protected CounterCategoryAttribute(string name, string description)
    {
      Name = name;
      Description = description;
    }

    public readonly string Name;
    public readonly string Description;

    public override string ToString() => Name;

    public abstract CounterCategory GetCategory();
  }
}