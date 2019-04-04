using System.Threading.Tasks;
using EventStore.ClientAPI;
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
    readonly CatchUpSubscriptionSettings _settings;
    readonly TimelinePosition _checkpoint;
    readonly ITimelineObserver _observer;
    EventStoreCatchUpSubscription _subscription;

    public TimelineSubscription(
      EventStoreContext context,
      CatchUpSubscriptionSettings settings,
      TimelinePosition checkpoint,
      ITimelineObserver observer)
    {
      _context = context;
      _settings = settings;
      _checkpoint = checkpoint;
      _observer = observer;
    }

    protected override Task Open()
    {
      _subscription = _context.Connection.SubscribeToStreamFrom(
        TimelineStreams.Timeline,
        _checkpoint.ToInt64OrNull(),
        _settings,
        eventAppeared: (_, e) =>
          _observer.OnNext(_context.ReadAreaPoint(e)),
        subscriptionDropped: (_, reason, error) =>
          _observer.OnDropped(reason.ToString(), error));

      return base.Open();
    }

    protected override Task Close()
    {
      _subscription?.Stop();

      return base.Close();
    }
  }
}