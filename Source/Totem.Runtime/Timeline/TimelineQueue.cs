using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A queue of timeline points with asynchronous dequeues for the whole timeline
	/// </summary>
	internal sealed class TimelineQueue : Connection
	{
		private readonly Subject<TimelineMessage> _messages = new Subject<TimelineMessage>();
		private readonly SortedDictionary<TimelinePosition, TimelineMessage> _futureMessages = new SortedDictionary<TimelinePosition, TimelineMessage>();
		private readonly TimelineSchedule _schedule;
		private readonly TimelineFlowSet _flows;
		private readonly TimelineRequestSet _requests;
		private TimelinePosition _nextPosition;

		internal TimelineQueue(TimelineSchedule schedule, TimelineFlowSet flows, TimelineRequestSet requests)
		{
			_schedule = schedule;
			_flows = flows;
			_requests = requests;
		}

		protected override void Open()
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

		internal void ResumeWith(ResumeInfo info)
		{
			_nextPosition = info.NextPosition;

			foreach(var pointInfo in info.Points)
			{
				_messages.OnNext(pointInfo.Message);
			}
		}

		internal void Enqueue(TimelineMessage message)
		{
			State.ExpectConnected();

			lock(_messages)
			{
				EnqueueNowOrInFuture(message);
			}
		}

		internal void Enqueue(IEnumerable<TimelineMessage> messages)
		{
			State.ExpectConnected();

			lock(_messages)
			{
				foreach(var message in messages)
				{
					EnqueueNowOrInFuture(message);
				}
			}
		}

		private void EnqueueNowOrInFuture(TimelineMessage message)
		{
			ExpectNotInPast(message);

			if(IsNext(message))
			{
				EnqueueNext(message);

				EnqueueFutureMessages();
			}
			else
			{
				AddFutureMessage(message);
			}
		}

		private void ExpectNotInPast(TimelineMessage message)
		{
			if(message.Point.Position < _nextPosition)
			{
				throw new InvalidOperationException($"Cannot enqueue previously-pushed position {message.Point.Position}; expected {_nextPosition}");
			}
		}

		private bool IsNext(TimelineMessage message)
		{
			return message.Point.Position == _nextPosition;
		}

		private void EnqueueNext(TimelineMessage message)
		{
			_nextPosition = message.Point.Position.Next();

			_messages.OnNext(message);
		}

		private void EnqueueFutureMessages()
		{
			foreach(var futureMessage in _futureMessages.ToList())
			{
				var position = futureMessage.Key;
				var message = futureMessage.Value;

				if(position != _nextPosition)
				{
					break;
				}

				_futureMessages.Remove(position);

				_nextPosition = position.Next();

				_messages.OnNext(message);
			}
		}

		private void AddFutureMessage(TimelineMessage message)
		{
			_futureMessages.Add(message.Point.Position, message);
		}
	}
}