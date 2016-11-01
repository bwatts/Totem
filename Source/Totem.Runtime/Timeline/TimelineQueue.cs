using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A queue of timeline points with asynchronous dequeues for the whole timeline
	/// </summary>
	internal sealed class TimelineQueue : Connection
	{
    private readonly Subject<TimelineMessage> _messages = new Subject<TimelineMessage>();
    private readonly ITimelineDb _db;
    private readonly TimelineSchedule _schedule;
    private readonly TimelineFlowSet _flows;
		private readonly TimelineRequestSet _requests;
    private readonly EnqueueTransactionSet _transactions;

    internal TimelineQueue(
      ITimelineDb db,
      TimelineSchedule schedule,
      TimelineFlowSet flows,
      TimelineRequestSet requests)
		{
      _db = db;
			_schedule = schedule;
			_flows = flows;
			_requests = requests;

      _transactions = new EnqueueTransactionSet(this);
		}

		protected override void Open()
		{
      ObserveMessages();

      RunResume();
    }

    private void ObserveMessages()
    {
      Track(_messages
        .ObserveOn(ThreadPoolScheduler.Instance)
        .Subscribe(message =>
        {
          _schedule.Push(message);
          _flows.Push(message);
          _requests.Push(message);
        }));
    }

    private void RunResume()
    {
      Task.Run((Action) Resume, State.CancellationToken);
    }

    private void Resume()
    {
      var batchCount = 0;
      var pointCount = 0;
      var firstPosition = TimelinePosition.None;
      var lastPosition = TimelinePosition.None;

      try
      {
        foreach(var batch in _db.ReadResumeInfo())
        {
          batchCount += 1;
          pointCount += batch.Points.Count;

          if(firstPosition == TimelinePosition.None)
          {
            firstPosition = batch.FirstPosition;
          }

          lastPosition = batch.LastPosition;

          _schedule.ResumeWith(batch);

          foreach(var point in batch.Points)
          {
            _messages.OnNext(point.Message);
          }

          Log.Verbose(
            "[timeline] Resuming a batch of {Size} spanning {First:l}-{Last:l}",
            batch.Points.Count,
            batch.FirstPosition,
            batch.LastPosition);
        }

        if(pointCount > 0)
        {
          Log.Verbose(
            "[timeline] Resumed activity with {Batches:l} ({Points:l}) spanning {First:l}-{Last:l}",
            Text.Count(batchCount, "batch", "batches"),
            Text.Count(pointCount, "point"),
            firstPosition,
            lastPosition);
        }
      }
      catch(Exception error)
      {
        Log.Error(error, "[timeline] HALTED; failed to resume activity");
      }
    }

    //
    // Enqueueing
    //

    internal EnqueueTransaction StartEnqueue()
    {
      lock(_transactions)
      {
        return _transactions.Start();
      }
    }

    internal void RollbackEnqueue(EnqueueTransaction transaction)
    {
      lock(_transactions)
      {
        _transactions.Rollback(transaction);
      }
    }

    internal void CommitEnqueue(EnqueueTransaction transaction)
    {
      lock(_transactions)
      {
        _transactions.Commit(transaction);
      }
    }

    /// <summary>
    /// The set of transactions to enqueue messages on the timeline
    /// </summary>
    private sealed class EnqueueTransactionSet : Notion
    {
      internal readonly List<EnqueueTransaction> _open = new List<EnqueueTransaction>();
      internal readonly List<EnqueueTransaction> _concurrent = new List<EnqueueTransaction>();
      internal readonly List<TimelineMessage> _nextMessages = new List<TimelineMessage>();
      private readonly TimelineQueue _queue;

      internal EnqueueTransactionSet(TimelineQueue queue)
      {
        _queue = queue;
      }

      internal EnqueueTransaction Start()
      {
        var transaction = new EnqueueTransaction(_queue);

        _open.Add(transaction);

        return transaction;
      }

      internal void Rollback(EnqueueTransaction transaction)
      {
        _open.Remove(transaction);
        _concurrent.Remove(transaction);

        if(_open.Count == 0 && _concurrent.Count == 0 && _nextMessages.Count > 0)
        {
          PushMessages(_nextMessages.RemoveAll());
        }
      }

      internal void Commit(EnqueueTransaction transaction)
      {
        _open.Remove(transaction);

        if(_open.Count == 0 && _concurrent.Count == 0)
        {
          PushMessages(transaction);
        }
        else if(_concurrent.Count == 0)
        {
          _nextMessages.AddRange(transaction);

          _concurrent.AddRange(_open);

          _open.Clear();
        }
        else
        {
          _nextMessages.AddRange(transaction);

          _concurrent.Remove(transaction);

          if(_concurrent.Count == 0)
          {
            PushMessages(_nextMessages.RemoveAll());
          }
        }
      }

      private void PushMessages(IEnumerable<TimelineMessage> messages)
      {
        foreach(var message in messages.OrderBy(m => m.Point.Position))
        {
          _queue._messages.OnNext(message);
        }
      }
    }

    /// <summary>
    /// The intent to enqueue one or more messages on the timeline
    /// </summary>
    internal sealed class EnqueueTransaction : IDisposable, IEnumerable<TimelineMessage>
    {
      private readonly TimelineQueue _queue;
      private TimelineMessage _message;
      private Many<TimelineMessage> _messages;

      internal EnqueueTransaction(TimelineQueue queue)
      {
        _queue = queue;
      }

      internal void Commit(TimelineMessage message)
      {
        _message = message;

        _queue.CommitEnqueue(this);
      }

      internal void Commit(Many<TimelineMessage> messages)
      {
        _messages = messages;

        _queue.CommitEnqueue(this);
      }

      public void Dispose()
      {
        if(_message == null && _messages == null)
        {
          _queue.RollbackEnqueue(this);
        }
      }

      public IEnumerator<TimelineMessage> GetEnumerator()
      {
        if(_message != null)
        {
          yield return _message;
        }
        else
        {
          foreach(var message in _messages)
          {
            yield return message;
          }
        }
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
  }
}