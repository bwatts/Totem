using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Timeline.Client;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// A subscription to the changed query stream in EventStore
  /// </summary>
  internal class QueryChangedSubscription : Notion
  {
    readonly QueryDb _db;
    readonly EventStoreContext _context;
    EventStoreSubscription _subscription;
    int _subscribeLock;

    internal QueryChangedSubscription(QueryDb db, EventStoreContext context)
    {
      _db = db;
      _context = context;
    }

    internal async Task EnsureSubscribed()
    {
      if(_subscription == null && AcquireSubscribeLock())
      {
        try
        {
          _subscription = await SubscribeToStream();
        }
        finally
        {
          ReleaseSubscribeLock();
        }
      }
    }

    internal void Unsubscribe() =>
      Interlocked.Exchange(ref _subscription, null)?.Dispose();

    bool AcquireSubscribeLock() =>
      Interlocked.CompareExchange(ref _subscribeLock, 1, 0) == 0;

    void ReleaseSubscribeLock() =>
      Interlocked.Exchange(ref _subscribeLock, 0);

    async Task<EventStoreSubscription> SubscribeToStream()
    {
      var task = _context.Connection.SubscribeToStreamAsync(
        TimelineStreams.ChangedQueries,
        resolveLinkTos: false,
        eventAppeared: (_, e) => OnChanged(e),
        subscriptionDropped: (_, reason, error) => OnDropped(reason, error));

      return await task.ConfigureAwait(false);
    }

    void OnChanged(ResolvedEvent e) =>
      _db.OnChanged(QueryETag.From(
        _context.Json.FromJsonUtf8<QueryChanged>(e.Event.Data).ETag,
        _context.Area));

    void OnDropped(SubscriptionDropReason reason, Exception error)
    {
      if(error != null)
      {
        Log.Error(error, "Dropped changed query subscription ({Reason})", reason);
      }
      else
      {
        Log.Debug("Dropped changed query subscription ({Reason})", reason);
      }
    }
  }
}