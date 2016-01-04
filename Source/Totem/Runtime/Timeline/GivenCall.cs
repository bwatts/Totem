using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A call to a Given method of a <see cref="Timeline.Flow"/>
  /// </summary>
  public class GivenCall : FlowCall
	{
		internal GivenCall(Flow flow, TimelinePoint point, FlowGiven given) : base(flow, point)
		{
      Given = given;
		}

    public readonly FlowGiven Given;

    public override string ToString() => $"{Flow}.Given({Point})";

    public void Make()
    {
      Given.Call(Flow, Point.Event);
    }
  }
}