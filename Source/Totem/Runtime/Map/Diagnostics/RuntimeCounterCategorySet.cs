using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A set of performance counter categories, indexed by name
  /// </summary>
  public class RuntimeCounterCategorySet : RuntimeSet<string, RuntimeCounterCategory>
  {
    internal override string GetKey(RuntimeCounterCategory value)
    {
      return value.Declaration.Name;
    }
  }
}