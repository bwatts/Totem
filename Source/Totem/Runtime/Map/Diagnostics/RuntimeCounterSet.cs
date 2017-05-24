using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A set of performance counters, indexed by name
  /// </summary>
  public sealed class RuntimeCounterSet : RuntimeSet<string, RuntimeCounter>
  {
    internal override string GetKey(RuntimeCounter value)
    {
      return value.Declaration.Name;
    }
  }
}