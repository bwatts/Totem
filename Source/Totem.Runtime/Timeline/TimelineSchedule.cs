using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Schedules points on the timeline to occur in the future
	/// </summary>
	internal sealed class TimelineSchedule : Connection
	{
		private readonly HashSet<TimelinePosition> _resumedPositions = new HashSet<TimelinePosition>();
		private readonly TimelineScope _scope;
		private TimelinePosition _resumeCheckpoint;

		internal TimelineSchedule(TimelineScope scope)
		{
			_scope = scope;
		}

		internal void ResumeWith(ResumeInfo info)
		{
			foreach(var pointInfo in info.Points)
			{
				if(pointInfo.OnSchedule)
				{
					_resumedPositions.Add(pointInfo.Point.Position);
				}

				_resumeCheckpoint = pointInfo.Point.Position;
			}
		}

		protected override void Close()
		{
			base.Close();

			_resumedPositions.Clear();
		}

		internal void Push(TimelinePoint point)
		{
			if(point.Scheduled && CanPush(point))
			{
				AddTimer(point);
			}
		}

		private bool CanPush(TimelinePoint point)
		{
			return _resumedPositions.Remove(point.Position) || point.Position > _resumeCheckpoint;
		}

		private void AddTimer(TimelinePoint point)
		{
			Observable
				.Timer(new DateTimeOffset(point.Event.When))
				.Take(1)
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Subscribe(_ => PushScheduled(point));
		}

		private void PushScheduled(TimelinePoint point)
		{
			if(State.IsConnecting || State.IsConnected)
			{
				_scope.PushScheduled(point);
			}
			else
			{
				Log.Warning("[timeline] Cannot push to schedule when {Phase:l} - ignoring {Point:l}", State.Phase, point);
			}
		}
	}
}