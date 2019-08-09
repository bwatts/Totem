using System;
using System.Threading.Tasks;
using Totem.Threading;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An application applying a test to a query type
  /// </summary>
  internal sealed class QueryApp
  {
    readonly QueryAppTimelineDb _timelineDb;
    Id _changeWaitId;
    TimelinePosition _changeWaitPosition;
    TimedWait _changeWait;
    Query _changedQuery;

    public QueryApp(QueryAppTimelineDb timelineDb)
    {
      _timelineDb = timelineDb;

      timelineDb.SubscribeApp(this);
    }

    internal Task Append(Event e) =>
      _timelineDb.WriteFromApp(e);

    internal async Task<TQuery> AppendAndGet<TQuery>(Id queryId, Event e, ExpectTimeout changeTimeout) where TQuery : Query
    {
      _changeWaitId = queryId;
      _changeWaitPosition = await _timelineDb.WriteFromApp(e);
      
      try
      {
        // The timeline might call OnCheckpoint before _changeWaitPosition gets assigned, in which case we store
        // the instance and do an eager check here before waiting.

        if(_changedQuery == null || _changedQuery.Context.CheckpointPosition != _changeWaitPosition)
        {
          _changeWait = changeTimeout.ToTimedWait();

          await _changeWait.Task;
        }

        if(_changedQuery.Context.ErrorPosition.IsSome)
        {
          throw new Exception($"Query {_changedQuery} stopped with the following error: {_changedQuery.Context.ErrorMessage}");
        }

        return (TQuery) _changedQuery;
      }
      finally
      {
        _changeWaitId = Id.Unassigned;
        _changeWaitPosition = TimelinePosition.None;
        _changeWait = null;
        _changedQuery = null;
      }
    }

    internal void OnCheckpoint(Query query)
    {
      if(query.Context.Key.Id == _changeWaitId)
      {
        _changedQuery = query;

        if(_changeWait != null && query.Context.CheckpointPosition == _changeWaitPosition)
        {
          _changeWait.OnOccurred();
        }
      }
    }
  }
}