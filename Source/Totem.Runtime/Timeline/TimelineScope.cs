using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A scope in which timeline points occur synchronously
	/// </summary>
	public abstract class TimelineScope : Connection, ITimelineScope
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

		public void Push(TimelinePoint point)
		{
			if(State.IsConnecting || State.IsConnected)
			{
				_points.OnNext(point);
			}
			else
			{
				Log.Warning("[timeline] Cannot push to scope when {Phase:l} - ignoring {Point:l}", State.Phase, point);
			}
		}

		private void WhenPushed(TimelinePoint point)
		{
			Point = point;

			try
			{
				CallWhen().Wait(State.CancellationToken);
			}
			catch(OperationCanceledException error)
			{
				Log.Info("[timeline] When call cancelled for point {Point:l}", Point);
			}
			finally
			{
				Point = null;
			}
		}

		protected abstract Task CallWhen();
	}
}