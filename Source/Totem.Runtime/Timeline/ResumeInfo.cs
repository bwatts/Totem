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
		public ResumeInfo(TimelinePosition nextPosition, Many<ResumePoint> points = null)
		{
			Expect(nextPosition.IsSome, "Cannot resume without a next position");

			NextPosition = nextPosition;
			Points = points ?? new Many<ResumePoint>();
		}

		public readonly TimelinePosition NextPosition;
		public readonly Many<ResumePoint> Points;

		public override Text ToText() => Text
			.Of(NextPosition)
			.Write(" ")
			.WriteInParentheses(Text.Count(Points.Count, "point"));
	}
}