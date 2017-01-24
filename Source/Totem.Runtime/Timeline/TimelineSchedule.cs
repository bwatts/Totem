using System;
using System.Collections.Concurrent;
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
    readonly ConcurrentDictionary<IDisposable, bool> _timers = new ConcurrentDictionary<IDisposable, bool>();
    readonly TimelineScope _timeline;

    internal TimelineSchedule(TimelineScope timeline)
		{
			_timeline = timeline;
		}

    protected override void Close()
    {
      foreach(var timer in _timers.Keys)
      {
        timer.Dispose();
      }
    }

    internal void Push(TimelineMessage message)
		{
      var timer = null as IDisposable;

      timer = Observable
				.Timer(new DateTimeOffset(message.Point.Event.When))
				.Take(1)
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Subscribe(_ => PushToTimeline(message, timer));

      _timers[timer] = true;
		}

    void PushToTimeline(TimelineMessage message, IDisposable timer)
    {
      try
      {
        bool ignored;

        _timers.TryRemove(timer, out ignored);

        timer.Dispose();

        _timeline.PushFromSchedule(message);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to push scheduled event of type {EventType}. The timeline will attempt to push it again after a restart.", message.Point.EventType);
      }
    }
	}
}