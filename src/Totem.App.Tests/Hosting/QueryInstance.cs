using System;
using System.Threading.Tasks;
using Totem.Threading;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An instance of a query type under test
  /// </summary>
  internal sealed class QueryInstance
  {
    readonly object _updateLock = new object();
    readonly FlowKey _key;
    TimelinePosition _latestAppended;
    TimedWait _pendingGet;
    TimedWait _pendingDone;
    Query _query;

    internal QueryInstance(FlowKey key)
    {
      _key = key;
    }

    internal void OnAppended(TimelinePoint point)
    {
      lock(_updateLock)
      {
        _latestAppended = point.Position;
      }
    }

    internal async Task<TQuery> Get<TQuery>(ExpectTimeout timeout) where TQuery : Query
    {
      var pendingGet = null as TimedWait;

      lock(_updateLock)
      {
        if(IsInitial)
        {
          return (TQuery) _key.Type.New();
        }

        if(IsLatest)
        {
          return (TQuery) _query;
        }

        pendingGet = timeout.ToTimedWait();

        _pendingGet = pendingGet;
      }

      await pendingGet.Task;

      _pendingGet = null;

      return (TQuery) _query;
    }

    internal async Task ExpectDone(ExpectTimeout timeout)
    {
      var pendingDone = null as TimedWait;

      lock(_updateLock)
      {
        if(IsLatest && IsDone)
        {
          return;
        }

        pendingDone = timeout.ToTimedWait();

        _pendingDone = pendingDone;
      }

      await pendingDone.Task;

      _pendingDone = null;
    }

    internal void OnCheckpoint(Query query)
    {
      lock(_updateLock)
      {
        _query = query;

        if(_pendingGet != null)
        {
          TryResolveGet();
        }

        if(_pendingDone != null)
        {
          TryResolveDone();
        }
      }
    }

    //
    // Details
    //

    bool IsInitial => _query == null && _latestAppended.IsNone;
    bool IsLatest => _query != null && _query.Context.CheckpointPosition == _latestAppended;
    bool IsDone => _query.Context.IsDone;
    bool HasError => _query.Context.ErrorPosition.IsSome;

    void TryResolveGet()
    {
      if(HasError)
      {
        _pendingGet.OnError(new Exception(_query.Context.ErrorMessage));
      }
      else if(IsDone)
      {
        _pendingGet.OnError(new Exception($"Expected to get {_key} but it is done"));
      }
      else
      {
        if(IsLatest)
        {
          _pendingGet.OnOccurred();
        }
      }
    }

    void TryResolveDone()
    {
      if(HasError)
      {
        _pendingDone.OnError(new Exception(_query.Context.ErrorMessage));
      }
      else
      {
        if(IsLatest && IsDone)
        {
          _pendingDone.OnOccurred();
        }
      }
    }
  }
}