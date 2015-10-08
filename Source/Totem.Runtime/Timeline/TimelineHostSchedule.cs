using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Schedules points on the timeline to occur in the future
	/// </summary>
	internal sealed class TimelineHostSchedule
	{
		private readonly TimelineHost _host;
		private ConcurrentDictionary<TimelinePosition, bool> _resumeSchedule;
    private TimelinePosition _resumeCheckpoint;

		internal TimelineHostSchedule(TimelineHost host)
		{
			_host = host;
		}

		internal void ResumeWith(TimelineResumeInfo info)
		{
			_resumeSchedule = new ConcurrentDictionary<TimelinePosition, bool>();

			foreach(var point in info.Points)
			{
				if(point.OnSchedule)
				{
					_resumeSchedule.TryAdd(point.Point.Position, true);
				}

				_resumeCheckpoint = point.Point.Position;
			}
		}

		internal void TryPush(TimelinePoint point)
		{
			if(CanPush(point))
			{
				Push(point);
			}
		}

		private bool CanPush(TimelinePoint point)
		{
			if(!point.Scheduled)
			{
				return false;
			}

			bool ignored;

			return point.Position > _resumeCheckpoint || _resumeSchedule.TryGetValue(point.Position, out ignored);
    }

		private void Push(TimelinePoint point)
		{
			var when = new DateTimeOffset(point.Event.When);

			// Embed subscription lifetime into the callback

			IDisposable subscription = null;

			subscription = Observable.Timer(when).Take(1).Subscribe(_ =>
			{
				if(subscription != null)
				{
					subscription.Dispose();
				}

				_host.PushOccurred(point);
			});
		}
	}
}