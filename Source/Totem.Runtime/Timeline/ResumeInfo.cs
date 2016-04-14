using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The information necessary to resume the timeline
	/// </summary>
	public class ResumeInfo
	{
		public ResumeInfo(Many<ResumePoint> points)
		{
			Points = points;
		}

		public readonly Many<ResumePoint> Points;

    public void Push(PushScope scope)
    {
      foreach(var point in Points)
      {
        point.Push(scope);
      }
    }
	}
}