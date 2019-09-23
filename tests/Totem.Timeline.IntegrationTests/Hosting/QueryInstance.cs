using System;
using System.Threading.Tasks;
using Totem.App.Tests;
using Totem.Threading;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// An instance of a query type under test
  /// </summary>
  internal sealed class QueryInstance
  {
    readonly object _updateLock = new object();
    TimelinePosition _latestAppended;
    TimelinePosition _checkpoint;
    TimedWait _latestWait;

    bool IsLatest => _checkpoint == _latestAppended;

    internal async Task WaitForLatest(ExpectTimeout timeout)
    {
      var latestWait = null as TimedWait;

      lock(_updateLock)
      {
        if(IsLatest)
        {
          return;
        }

        latestWait = timeout.ToTimedWait();

        _latestWait = latestWait;
      }

      await latestWait.Task;

      _latestWait = null;
    }

    internal void OnAppended(TimelinePosition position)
    {
      lock(_updateLock)
      {
        _latestAppended = position;
      }
    }

    internal void OnChanged(TimelinePosition position)
    {
      lock(_updateLock)
      {
        _checkpoint = position;

        if(IsLatest)
        {
          _latestWait?.OnOccurred();
        }
      }
    }

    internal void OnError(Exception error)
    {
      lock(_updateLock)
      {
        _latestWait?.OnError(error);
      }
    }
  }
}