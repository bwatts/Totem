using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Schedules and synchronizes events on the timeline event loop.
  /// </summary>
  internal sealed class TimelineScheduler : IScheduler
  {
    readonly EventLoopScheduler _eventLoop;

    TimelineScheduler()
    {
      _eventLoop = new EventLoopScheduler(start => new Thread(start)
      {
        IsBackground = true,
        Name = "Timeline Event Loop"
      });
    }

    public DateTimeOffset Now => _eventLoop.Now;

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
      return _eventLoop.Schedule(state, action);
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      return _eventLoop.Schedule(state, dueTime, action);
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
      return _eventLoop.Schedule(state, dueTime, action);
    }

    //
    // Instance
    //

    public static IScheduler Instance { get; } = new TimelineScheduler();
  }
}
