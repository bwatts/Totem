using System;
using System.Threading.Tasks;
using Totem.App.Tests;
using Totem.Threading;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Waits for query change notifications after appending events to the timeline
  /// </summary>
  internal sealed class QueryChangeWait
  {
    readonly IntegrationApp _app;
    readonly AreaMap _area;
    FlowKey _key;
    TimelinePosition _position;
    TimedWait _wait;
    QueryETag _changedETag;
    Exception _dropError;

    internal QueryChangeWait(IntegrationApp app, AreaMap area)
    {
      _app = app;
      _area = area;
    }

    internal async Task<TQuery> AppendAndGet<TQuery>(Id queryId, Event e, ExpectTimeout timeout) where TQuery : Query
    {
      if(_dropError != null)
      {
        throw new Exception($"Cannot append and get query because subscription dropped", _dropError);
      }

      _key = _area.Queries.Get<TQuery>().CreateKey(queryId);

      _position = await _app.Append(e);

      try
      {
        // The timeline might call OnChanged before _position gets assigned, in which case we store the
        // ETag and do an eager check here before waiting.

        if(_changedETag == null || _changedETag.Checkpoint != _position)
        {
          _wait = timeout.ToTimedWait();

          await _wait.Task;
        }

        return await _app.Get<TQuery>(queryId);
      }
      finally
      {
        _key = null;
        _position = TimelinePosition.None;
        _wait = null;
        _changedETag = null;
      }
    }

    internal void OnChanged(QueryETag query)
    {
      if(query.Key == _key)
      {
        _changedETag = query;

        if(_wait != null && query.Checkpoint == _position)
        {
          _wait.OnOccurred();
        }
      }
    }

    internal void OnStopped(QueryETag query, Exception error)
    {
      if(query.Key == _key)
      {
        _wait?.OnError(new Exception($"Query {query} stopped", error));
      }
    }

    internal void OnDropped(Exception error)
    {
      var wait = _wait;

      if(wait != null)
      {
        wait.OnError(error);
      }
      else
      {
        _dropError = error;
      }
    }
  }
}