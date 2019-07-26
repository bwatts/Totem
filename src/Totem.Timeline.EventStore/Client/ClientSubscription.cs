using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Totem.Runtime.Json;
using Totem.Timeline.Client;

namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// A subscription to the client stream in EventStore
  /// </summary>
  internal sealed class ClientSubscription : IDisposable
  {
    readonly EventStoreContext _context;
    readonly IClientObserver _observer;
    EventStoreSubscription _timelineSubscription;
    EventStoreSubscription _clientSubscription;

    internal ClientSubscription(EventStoreContext context, IClientObserver observer)
    {
      _context = context;
      _observer = observer;
    }

    public void Dispose()
    {
      _timelineSubscription?.Dispose();
      _clientSubscription?.Dispose();
    }

    internal async Task Subscribe()
    {
      await SubscribeToTimeline();
      await SubscribeToClient();
    }

    async Task SubscribeToTimeline()
    {
      var task = _context.Connection.SubscribeToStreamAsync(
        TimelineStreams.Timeline,
        resolveLinkTos: false,
        eventAppeared: (_, e) => OnNextFromTimeline(e),
        subscriptionDropped: (_, reason, error) => OnDropped(reason, error));

      _timelineSubscription = await task.ConfigureAwait(false);
    }

    async Task SubscribeToClient()
    {
      var task = _context.Connection.SubscribeToStreamAsync(
        TimelineStreams.Client,
        resolveLinkTos: false,
        eventAppeared: (_, e) => OnNextFromClient(e),
        subscriptionDropped: (_, reason, error) => OnDropped(reason, error));

      _clientSubscription = await task.ConfigureAwait(false);
    }

    Task OnNextFromTimeline(ResolvedEvent e) =>
      _observer.OnNext(_context.ReadAreaPoint(e));

    void OnDropped(SubscriptionDropReason reason, Exception error) =>
      _observer.OnDropped(reason.ToString(), error);

    Task OnNextFromClient(ResolvedEvent e)
    {
      switch(e.Event.EventType)
      {
        case "timeline:CommandFailed":
          return OnNext(ReadEvent<CommandFailed>(e));
        case "timeline:QueryChanged":
          return OnNext(ReadEvent<QueryChanged>(e));
        case "timeline:QueryStopped":
          return OnNext(ReadEvent<QueryStopped>(e));
        default:
          return Task.CompletedTask;
      }
    }

    T ReadEvent<T>(ResolvedEvent e) =>
      _context.Json.FromJsonUtf8<T>(e.Event.Data);

    Task OnNext(CommandFailed e) =>
      _observer.OnCommandFailed(e.CommandId, e.Error);

    Task OnNext(QueryChanged e) =>
      _observer.OnQueryChanged(ETagFrom(e.ETag));

    Task OnNext(QueryStopped e) =>
      _observer.OnQueryStopped(ETagFrom(e.ETag), e.Error);

    QueryETag ETagFrom(string etag) =>
      QueryETag.From(etag, _context.Area);
  }
}