using System;
using System.Linq;
using System.Reactive.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Schedules points on the timeline to occur in the future
  /// </summary>
  internal sealed class TimelineSchedule : Connection
  {
    readonly TimelineScope _timeline;

    internal TimelineSchedule(TimelineScope timeline)
    {
      _timeline = timeline;
    }

    internal void Push(TimelineMessage message)
    {
      var timer = null as IDisposable;

      timer = Observable
        .Timer(new DateTimeOffset(message.Point.Event.When))
        .Take(1)
        .Subscribe(_ => PushToTimeline(message, timer));
    }

    void PushToTimeline(TimelineMessage message, IDisposable timer)
    {
      try
      {
        timer?.Dispose();
        _timeline.PushFromSchedule(message);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to push scheduled event of type {EventType}. The timeline will attempt to push it again after a restart.", message.Point.EventType);
      }
    }
  }
}