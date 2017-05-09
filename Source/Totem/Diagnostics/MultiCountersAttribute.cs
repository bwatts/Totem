﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Indicates counters in the decorated class belong to the specified category and
  /// have multiple instances
  /// </summary>
  public sealed class MultiCountersAttribute : CounterCategoryAttribute
  {
    public MultiCountersAttribute(string category, string description)
      : base(category, description)
    {}

    public override CounterCategory GetCategory()
    {
      return CounterCategory.MultiInstance(Name, Description);
    }
  }
}