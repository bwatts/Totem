using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The information necessary to resume the timeline
	/// </summary>
	public class ResumeInfo : Notion
	{
		public ResumeInfo(Many<ResumePoint> points = null)
		{
			Points = points ?? new Many<ResumePoint>();
		}

		public readonly Many<ResumePoint> Points;

    public override Text ToText() => Text.Count(Points.Count, "point");
	}
}