using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamsDB.Driver;
using Totem.Runtime.Json;
using Totem.Timeline.Client;

namespace Totem.Timeline.StreamsDb.Client
{
  /// <summary>
  /// A subscription to the client stream in EventStore
  /// </summary>
  internal sealed class ClientSubscription : IDisposable
  {
    readonly StreamsDbContext _context;
    readonly IClientObserver _observer;
    IStreamSubscription _timelineSubscription;
    IStreamSubscription _clientSubscription;

    internal ClientSubscription(StreamsDbContext context, IClientObserver observer)
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

    Task SubscribeToTimeline()
    {
      _timelineSubscription = _context.Client.DB().SubscribeStream(TimelineStreams.GetTimelineStream(_context.AreaName), 0);

      Task.Run(async () =>
      {
        do
        {
          await _timelineSubscription.MoveNext();
          await OnNextFromTimeline(_timelineSubscription.Current);
        }
        while (true);
      });

      return Task.CompletedTask;
    }

    Task SubscribeToClient()
    {
      _clientSubscription = _context.Client.DB().SubscribeStream(TimelineStreams.GetClientStream(_context.AreaName), 0);

      Task.Run(async () =>
      {
        do
        {
          await _clientSubscription.MoveNext();
          await OnNextFromClient(_clientSubscription.Current);
        }
        while (true);
      });

      return Task.CompletedTask;
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