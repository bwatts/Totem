using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A scope in which timeline points occur synchronously
	/// </summary>
	public abstract class PushScope : Connection
	{
		private Subject<TimelinePoint> _points;

		protected TimelinePoint Point { get; private set; }

		protected override void Open()
		{
			_points = new Subject<TimelinePoint>();

			Track(_points);

			Track(_points
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Subscribe(WhenPushed));
		}

		protected override void Close()
		{
			_points = null;
		}

    private void WhenPushed(TimelinePoint point)
    {
      Point = point;

      try
      {
        Push();
      }
      finally
      {
        Point = null;
      }
    }

    protected abstract void Push();

    public void Push(TimelinePoint point)
		{
      if(State.IsConnecting || State.IsConnected)
      {
        _points.OnNext(point);
      }
      else
      {
        Log.Warning(
          "[timeline] Cannot push to scope in phase {Phase} - ignoring {Point:l}",
          State.Phase,
          point);
      }
    }

    public void Push(IEnumerable<TimelinePoint> points)
    {
      if(State.IsConnecting || State.IsConnected)
      {
        foreach(var point in points)
        {
          _points.OnNext(point);
        }
      }
      else
      {
        Log.Warning(
          "[timeline] Cannot push to scope in phase {Phase} - ignoring {Point:l}",
          State.Phase,
          Text.Count(points.Count(), "point"));
      }
    }

    public void Push(params TimelinePoint[] points)
    {
      Push(points.AsEnumerable());
    }
	}
}