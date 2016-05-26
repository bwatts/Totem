using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A database persisting timeline data in the local runtime
	/// </summary>
	public sealed class LocalTimelineDb : Notion, ITimelineDb
	{
		private long _position = -1;

		public ResumeInfo ReadResumeInfo()
		{
			return new ResumeInfo(new TimelinePosition(0));
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
			var newPoint = new TimelinePoint(cause, NextPosition(), type, e, scheduled);

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

		private TimelinePosition NextPosition()
		{
			return new TimelinePosition(Interlocked.Increment(ref _position));
		}
	}
}