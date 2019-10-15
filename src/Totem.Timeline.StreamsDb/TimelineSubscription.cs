using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime;
using Totem.Timeline.Runtime;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// A resumable EventStore subscription to the hosted timeline area
  /// </summary>
  public class TimelineSubscription : Connection
  {
    readonly EventStoreContext _context;
    readonly TimelinePosition _checkpoint;
    readonly ITimelineObserver _observer;
    IStreamSubscription _subscription;
    CancellationTokenSource _cancellationTokenSource;

    public TimelineSubscription(
      EventStoreContext context,
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
      _subscription = _context.Client.DB().SubscribeStream(
        TimelineStreams.Timeline,
        _checkpoint.ToInt64()
      );

      Task.Run(async () =>
      {
        do
        {
          var hasNext = await _subscription.MoveNext();
          if (!hasNext)
          {
            await Task.Delay(1000);
          }

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