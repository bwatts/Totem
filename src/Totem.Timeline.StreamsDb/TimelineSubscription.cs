using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// A resumable EventStore subscription to the hosted timeline area
  /// </summary>
  public class TimelineSubscription : Connection
  {
    readonly StreamsDbContext _context;
    readonly TimelinePosition _checkpoint;
    readonly ITimelineObserver _observer;
    IStreamSubscription _subscription;
    CancellationTokenSource _cancellationTokenSource;

    public TimelineSubscription(
      StreamsDbContext context,
      TimelinePosition checkpoint,
      ITimelineObserver observer)
    {
      _context = context;
      _checkpoint = checkpoint;
      _observer = observer;

      _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override Task Open()
    {
      var position = _checkpoint.ToInt64OrNull();

      _subscription = _context.Client.DB().SubscribeStream(
        $"{_context.AreaName}-{TimelineStreams.Timeline}",
        position.HasValue ? position.Value + 1 : 0
      );

      Task.Run(async () =>
      {
        do
        {
          await _subscription.MoveNext();
          await _observer.OnNext(_context.ReadAreaPoint(_subscription.Current));
        }
        while (!_cancellationTokenSource.IsCancellationRequested);
      });

      return base.Open();
    }

    protected override Task Close()
    {
      _cancellationTokenSource.Cancel();

      return base.Close();
    }
  }
}