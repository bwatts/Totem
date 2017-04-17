using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Indicates counters in the decorated class belong to the specified category and
  /// are not partitioned into instances
  /// </summary>
  public sealed class SingleInstanceCategoryAttribute : CounterCategoryAttribute
  {
    public SingleInstanceCategoryAttribute(string category, string description)
      : base(category, description)
    {}

    public override CounterCategory GetCategory()
    {
      return CounterCategory.SingleInstance(Name, Description);
    }
  }
}