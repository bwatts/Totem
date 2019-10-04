using System;
using System.Threading.Tasks;
using Totem.Threading;
using Totem.Timeline;

namespace Totem.App.Tests.Hosting
{
  /// <summary>
  /// An instance of a topic type under test
  /// </summary>
  internal sealed class TopicInstance
  {
    readonly object _updateLock = new object();
    readonly FlowKey _key;
    TimelinePosition _latestAppended;
    TimedWait _pendingDone;
    Topic _topic;

    internal TopicInstance(FlowKey key)
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

    internal void OnCheckpoint(Topic topic)
    {
      lock(_updateLock)
      {
        _topic = topic;

        TryResolveDone();
      }
    }

    //
    // Details
    //

    bool IsLatest => _topic != null && _topic.Context.CheckpointPosition == _latestAppended;
    bool IsDone => _topic.Context.IsDone;
    bool HasError => _topic.Context.ErrorPosition.IsSome;

    void TryResolveDone()
    {
      if(HasError)
      {
        _pendingDone.OnError(new Exception(_topic.Context.ErrorMessage));
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