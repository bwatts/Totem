using System;
using System.Collections.Generic;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Declares a timeline area to reside in the assembly of the derived type
  /// </summary>
  public abstract class TimelineArea
  {
    public virtual IEnumerable<Type> GetTypes() =>
      GetType().Assembly.GetTypes();
  }
}