using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A database persisting timeline data in the local runtime
	/// </summary>
	public sealed class LocalTimelineDb : Notion, ITimelineDb
	{
		private readonly List<TimelinePoint> _points = new List<TimelinePoint>();

		public ResumeInfo ReadResumeInfo()
		{
			return new ResumeInfo();
		}

		public Many<TimelinePoint> Write(TimelinePosition cause, Many<Event> events)
		{
			return events.ToMany(e => Write(cause, e));
		}

		public TimelinePoint WriteScheduled(TimelinePoint point)
		{
			return Write(point.Position, point.Event);
		}

		internal TimelinePoint Write(TimelinePosition cause, Event e)
		{
			var type = Runtime.GetEvent(e.GetType());

			var scheduled = e as EventScheduled;

			if(scheduled != null)
			{
				type = Runtime.GetEvent(scheduled.Event.GetType());

				e = scheduled.Event;
			}

			return Write(cause, type, e, scheduled != null);
		}

		private TimelinePoint Write(TimelinePosition cause, EventType type, Event e, bool scheduled)
		{
			var newPoint = new TimelinePoint(cause, new TimelinePosition(_points.Count), type, e, scheduled);

			_points.Add(newPoint);

			if(scheduled)
			{
				Log.Info("[timeline] {Cause:l} @@ {When} ++ {EventType:l}", cause, e.When, type);
			}
			else
			{
				Log.Info("[timeline] {Cause:l} ++ {Point:l}", cause, newPoint);
			}

			return newPoint;
		}
	}
}