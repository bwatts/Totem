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

		public ResumeInfo()
		{
      Flows = new Many<Flow>();
      _batches = Enumerable.Empty<Batch>();
		}

    public ResumeInfo(Many<Flow> flows, IEnumerable<Batch> batches)
    {
      Flows = flows;
      _batches = batches;
    }

    public readonly Many<Flow> Flows;

    public IEnumerator<Batch> GetEnumerator() => _batches.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The set of flows with which to resume the timeline
    /// </summary>
    public class Flow
    {
      public Flow(FlowKey key, TimelinePosition checkpoint)
      {
        Key = key;
        Checkpoint = checkpoint;
      }

      public readonly FlowKey Key;
      public readonly TimelinePosition Checkpoint;
    }

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