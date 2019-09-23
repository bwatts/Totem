using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Totem.App.Tests;
using Totem.App.Tests.Hosting;
using Totem.Runtime;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// An application applying a timeline integration test
  /// </summary>
  internal sealed class IntegrationApp : Connection, IClientObserver, IDisposable
  {
    readonly ConcurrentDictionary<Id, bool> _appendedEventIds = new ConcurrentDictionary<Id, bool>();
    readonly ConcurrentDictionary<FlowKey, QueryInstance> _queriesByKey = new ConcurrentDictionary<FlowKey, QueryInstance>();
    readonly ExpectQueue _expectQueue = new ExpectQueue();
    readonly AreaMap _area;
    readonly EventStoreProcess _eventStoreProcess;
    readonly IClientDb _clientDb;

    public IntegrationApp(AreaMap area, EventStoreProcess eventStoreProcess, IClientDb clientDb)
    {
      _area = area;
      _eventStoreProcess = eventStoreProcess;
      _clientDb = clientDb;
    }

    protected override async Task Open()
    {
      Track(_eventStoreProcess);
      Track(_clientDb);

      // Tracking doesn't connect immediately - do so before subscribing
      await _eventStoreProcess.Connect(this);
      await _clientDb.Connect(this);

      Track(await _clientDb.Subscribe(this));
    }

    void IDisposable.Dispose()
    {
      // If the host fails to start, this class will not get an opportunity to shut down gracefully,
      // which could lead to an orphaned EventStore process.
      //
      // The container will call Dispose regardless of startup success, so we can clean it up.

      if(_eventStoreProcess.State.IsConnected)
      {
        Task.Run(_eventStoreProcess.Disconnect).GetAwaiter().GetResult();
      }
    }

    internal async Task<TimelinePosition> Append(Event e)
    {
      var eventId = Id.FromGuid();

      _appendedEventIds[eventId] = true;

      Event.Traits.EventId.Set(e, eventId);
      Event.Traits.CommandId.Set(e, Id.FromGuid());

      var position = await _clientDb.WriteEvent(e);

      // This is slightly hacky, as the client db calculates the routes too, but doesn't return them.

      var type = _area.Events[e.GetType()];

      foreach(var route in type.GetRoutes(e))
      {
        if(route.Type.IsQuery)
        {
          GetQueryInstance(route).OnAppended(position);
        }
      }

      return position;
    }

    internal Task<TEvent> Expect<TEvent>(ExpectTimeout timeout, bool scheduled) where TEvent : Event =>
      _expectQueue.Dequeue<TEvent>(timeout, scheduled);

    internal async Task<TQuery> GetQuery<TQuery>(Id instanceId, ExpectTimeout timeout) where TQuery : Query
    {
      var key = _area.Queries.Get<TQuery>().CreateKey(instanceId);

      await GetQueryInstance(key).WaitForLatest(timeout);

      return (TQuery) await _clientDb.ReadQueryContent(key);
    }

    QueryInstance GetQueryInstance(FlowKey key) =>
      _queriesByKey.GetOrAdd(key, _ => new QueryInstance());

    //
    // IClientObserver
    //

    Task IClientObserver.OnNext(TimelinePoint point)
    {
      if(!_appendedEventIds.TryRemove(point.EventId, out _))
      {
        _expectQueue.Enqueue(point);
      }

      return Task.CompletedTask;
    }

    Task IClientObserver.OnDropped(string reason, Exception error)
    {
      if(error != null)
      {
        var dropped = new Exception($"Subscription dropped with reason {reason}", error);

        _expectQueue.OnError(dropped);

        foreach(var query in _queriesByKey.Values)
        {
          query.OnError(dropped);
        }
      }

      return Task.CompletedTask;
    }

    Task IClientObserver.OnCommandFailed(Id commandId, string error)
    {
      var failed = new Exception($"Command {commandId} failed with the following error: {error}");

      _expectQueue.OnError(failed);

      return Task.CompletedTask;
    }

    Task IClientObserver.OnQueryChanged(QueryETag query)
    {
      GetQueryInstance(query.Key).OnChanged(query.Checkpoint);

      return Task.CompletedTask;
    }

    Task IClientObserver.OnQueryStopped(QueryETag query, string error)
    {
      GetQueryInstance(query.Key).OnError(new Exception($"Query {query} stopped with the following error: {error}"));

      return Task.CompletedTask;
    }
  }
}