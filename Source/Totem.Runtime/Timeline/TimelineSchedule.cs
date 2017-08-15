using System;
using System.Linq;
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
    readonly TimelineScope _timeline;

    internal TimelineSchedule(TimelineScope timeline)
		{
			_timeline = timeline;
		}

    internal void Push(TimelinePoint point)
		{
      var timer = null as IDisposable;

      timer = Observable
        .Timer(new DateTimeOffset(point.Event.When), TaskPoolScheduler.Default)
        .Take(1)
        .Subscribe(_ => Task.Run(() => PushToTimeline(point, timer)));
		}

    async Task PushToTimeline(TimelinePoint point, IDisposable timer)
    {
      try
      {
        timer?.Dispose();

        await _timeline.PushScheduled(point);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to push scheduled event of type {EventType}. The timeline will attempt to push it again after a restart.", point.EventType);
      }
    }
	}
}