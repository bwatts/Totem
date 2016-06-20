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
		private readonly TimelineScope _timeline;
		private TimelinePosition _resumeCheckpoint;

		internal TimelineSchedule(TimelineScope timeline)
		{
			_timeline = timeline;
		}

		internal void ResumeWith(ResumeInfo info)
		{
			foreach(var pointInfo in info.Points)
			{
				var position = pointInfo.Message.Point.Position;

				if(pointInfo.OnSchedule)
				{
					_resumedPositions.Add(position);
				}

				_resumeCheckpoint = position;
			}
		}

		protected override void Close()
		{
			base.Close();

			_resumedPositions.Clear();
		}

		internal void Push(TimelineMessage message)
		{
			if(message.Point.Scheduled && CanPush(message.Point.Position))
			{
				StartTimer(message);
			}
		}

		private bool CanPush(TimelinePosition position)
		{
			return _resumedPositions.Remove(position) || position > _resumeCheckpoint;
		}

		private void StartTimer(TimelineMessage message)
		{
			Observable
				.Timer(new DateTimeOffset(message.Point.Event.When))
				.Take(1)
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Subscribe(_ => PushFromSchedule(message));
		}

		private void PushFromSchedule(TimelineMessage message)
		{
			if(State.IsConnecting || State.IsConnected)
			{
				_timeline.PushFromSchedule(message);
			}
			else
			{
				Log.Warning("[timeline] Cannot push to schedule when {Phase:l} - ignoring {Point:l}", State.Phase, message.Point);
			}
		}
	}
}