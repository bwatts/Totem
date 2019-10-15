using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamsDB.Driver;
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
    IStreamSubscription _timelineSubscription;
    IStreamSubscription _clientSubscription;

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
      await Task.Run(async () =>
      {
        _timelineSubscription = _context.Client.DB().SubscribeStream(
          TimelineStreams.Timeline,
          0
        );

        do
        {
          if (!await _timelineSubscription.MoveNext())
          {
            await Task.Delay(1000);
            continue;
          }

          await OnNextFromTimeline(_timelineSubscription.Current);
        }
        while (true);
      });
    }

    async Task SubscribeToClient()
    {
      await Task.Run(async () =>
      {
        _clientSubscription = _context.Client.DB().SubscribeStream(
          TimelineStreams.Client,
          0
        );

        do
        {
          if (!await _clientSubscription.MoveNext())
          {
            await Task.Delay(1000);
            continue;
          }

          await OnNextFromClient(_clientSubscription.Current);
        }
        while (true);
      });
    }

    Task OnNextFromTimeline(Message e) =>
      _observer.OnNext(_context.ReadAreaPoint(e));

    //void OnDropped(SubscriptionDropReason reason, Exception error) =>
    //  _observer.OnDropped(reason.ToString(), error);

    Task OnNextFromClient(Message e)
    {
      switch(e.Type)
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

    T ReadEvent<T>(Message e) =>
      _context.Json.FromJsonUtf8<T>(e.Value);

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