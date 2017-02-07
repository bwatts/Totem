using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Sets timers for points which occur in the future
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

    internal void Push(TimelinePoint point)
		{
      var timer = null as IDisposable;

      timer = Observable
        .Timer(new DateTimeOffset(point.Event.When))
        .Take(1)
        .SelectMany(_ => PushToTimeline(point, timer))
        .Subscribe();

      _timers[timer] = true;
		}

    async Task<Unit> PushToTimeline(TimelinePoint point, IDisposable timer)
    {
      try
      {
        bool ignored;

        _timers.TryRemove(timer, out ignored);

        timer.Dispose();

        await _timeline.PushScheduled(point);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to push scheduled event of type {EventType}. The timeline will attempt to push it again after a restart.", point.EventType);
      }

      return Unit.Default;
    }
	}
}