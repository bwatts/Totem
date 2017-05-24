using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Diagnostics;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A declared counter measuring an aspect of runtime performance
  /// </summary>
  public class RuntimeCounter
  {
    internal RuntimeCounter(RuntimeCounterCategory category, CounterBase declaration)
    {
      Category = category;
      Declaration = declaration;
    }

    public readonly RuntimeCounterCategory Category;
    public readonly CounterBase Declaration;

    public override string ToString() => Declaration.ToString();
  }
}