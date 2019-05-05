using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Totem.Runtime;

namespace Totem.Timeline.Runtime
{
  /// <summary>
  /// The set of scheduled events on the timeline
  /// </summary>
  internal class ScheduleHost : Notion
  {
    readonly ITimelineDb _db;

    internal ScheduleHost(ITimelineDb db)
    {
      _db = db;
    }

    internal void Resume(Many<TimelinePoint> schedule)
    {
      foreach(var point in schedule)
      {
        OnNext(point);
      }
    }

    internal Task OnNext(TimelinePoint point)
    {
      var whenOccurs = Event.GetWhenOccurs(point.Event);

      if(whenOccurs != null)
      {
        var timer = null as IDisposable;

        timer = Observable
          .Timer(whenOccurs.Value, TaskPoolScheduler.Default)
          .Take(1)
          .Subscribe(_ => Task.Run(() => WriteScheduledEvent(timer, point)));
      }

      return Task.CompletedTask;
    }

    async Task WriteScheduledEvent(IDisposable timer, TimelinePoint cause)
    {
      try
      {
        timer?.Dispose();

        await _db.WriteScheduledEvent(cause);
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] Failed to write scheduled event: {Cause}", cause);
      }
    }
  }
}