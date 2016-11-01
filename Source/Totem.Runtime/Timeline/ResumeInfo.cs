using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A series of points with which to resume the timeline
	/// </summary>
	public class ResumeInfo : Notion, IEnumerable<ResumeInfo.Batch>
	{
    readonly IEnumerable<Batch> _batches;

		public ResumeInfo(IEnumerable<Batch> batches = null)
		{
      _batches = batches ?? Enumerable.Empty<Batch>();
		}

    public IEnumerator<Batch> GetEnumerator() => _batches.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// A batch of points with which to resume the timeline
    /// </summary>
    public class Batch
    {
      public Batch(Many<Point> points)
      {
        Points = points;
        HasPoints = points.Any();

        if(HasPoints)
        {
          FirstPosition = Points.First().Message.Point.Position;
          LastPosition = Points.Last().Message.Point.Position;
        }
      }

      public readonly Many<Point> Points;
      public readonly bool HasPoints;
      public readonly TimelinePosition FirstPosition;
      public readonly TimelinePosition LastPosition;
    }

    /// <summary>
    /// A point with which to resume the timeline
    /// </summary>
    public class Point
    {
      public Point(TimelineMessage message, bool onSchedule)
      {
        Message = message;
        OnSchedule = onSchedule;
      }

      public readonly TimelineMessage Message;
      public readonly bool OnSchedule;

      public override string ToString() => Message.Point.ToString();
    }
  }
}