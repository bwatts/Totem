using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A call to a method defined by a <see cref="Timeline.Flow"/>
  /// </summary>
  public class FlowCall
  {
    internal FlowCall(Flow flow, TimelinePoint point)
    {
      Flow = flow;
      Point = point;
    }

    public readonly Flow Flow;
    public readonly TimelinePoint Point;
  }
}