using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Totem;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing and publishing to the timeline
	/// </summary>
	[Durable]
	public abstract class Flow : Notion
	{
		[Transient] private WhenCall _call;

		[Transient] protected FlowType FlowType { get; private set; }
		[Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
		[Transient] protected IDependencySource Dependencies { get; private set; }
		[Transient] protected ClaimsPrincipal Principal { get; private set; }
		[Transient] protected CancellationToken CancellationToken { get; private set; }

		public TimelinePosition Checkpoint { get { return Traits.Checkpoint.Get(this); } }
		public bool Done { get { return Traits.Done.Get(this); } private set { Traits.Done.Set(this, value); } }

		public void CallBefore(FlowBefore before, TimelinePoint point)
		{
			before.Call(this, point.Event);
		}

		public async Task CallWhen(WhenCall call)
		{
			StartWhenCall(call);

			try
			{
				await MakeWhenCall();
			}
			finally
			{
				EndWhenCall();
			}
		}

		private void StartWhenCall(WhenCall call)
		{
			Expect(_call).IsNull("Flow is already making a call");

			_call = call;
			FlowType = call.FlowType;
			Cause = call.Point.Cause;
			Event = call.Point.Event;
			EventType = call.Point.EventType;
			Dependencies = call.Dependencies;
			Principal = call.Principal;
			CancellationToken = call.CancellationToken;
		}

		protected virtual Task MakeWhenCall()
		{
			return FlowType.CallWhen(this, _call.Point, Dependencies);
		}

		private void EndWhenCall()
		{
			_call = null;
			FlowType = null;
			Cause = default(TimelinePosition);
			Event = null;
			EventType = null;
			Dependencies = null;
			Principal = null;
			CancellationToken = default(CancellationToken);
		}

		protected void ExpectMakingCall()
		{
			Expect(_call).IsNotNull("Flow is not making a When call");
		}

		//
		// Then
		//

		protected void ThenDone()
		{
			ExpectMakingCall();

			Expect(Done).IsFalse("Flow is already done");

			Done = true;
		}

		protected void Then(Event e)
		{
			ExpectMakingCall();

			_call.Publish(e);
		}

		protected void Then(IEnumerable<Event> events)
		{
			ExpectMakingCall();

			_call.Publish(events);
		}

		protected void Then(params Event[] events)
		{
			ExpectMakingCall();

			_call.Publish(events);
		}

		protected void ThenAt(DateTime whenOccurs, Event e)
		{
			ExpectMakingCall();

			_call.Schedule(whenOccurs, e);
		}

		protected void ThenAt(DateTime whenOccurs, IEnumerable<Event> events)
		{
			ExpectMakingCall();

			_call.Schedule(whenOccurs, events);
		}

		protected void ThenAt(DateTime whenOccurs, params Event[] events)
		{
			ExpectMakingCall();

			_call.Schedule(whenOccurs, events);
		}

		protected void ThenAt(TimeSpan timeOfDay, Event e)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), e);
		}

		protected void ThenAt(TimeSpan timeOfDay, IEnumerable<Event> events)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), events);
		}

		protected void ThenAt(TimeSpan timeOfDay, params Event[] events)
		{
			ThenAt(GetWhenOccursNext(timeOfDay), events);
		}

		private DateTime GetWhenOccursNext(TimeSpan timeOfDay)
		{
			// The time of day is relative to the client's timezone

			var now = Clock.Now.ToLocalTime();
			var today = now.Date;

			var whenToday = today + timeOfDay;

			var whenOccurs = whenToday > now
				? whenToday
				: today.AddDays(1) + timeOfDay;

			return whenOccurs.ToUniversalTime();
		}

		public new static class Traits
		{
			public static readonly Tag<TimelinePosition> Checkpoint = Tag.Declare(() => Checkpoint, TimelinePosition.None);
			public static readonly Tag<bool> Done = Tag.Declare(() => Done, false);
			public static readonly Tag<Id> RequestId = Tag.Declare(() => RequestId);

			public static Id EnsureRequestId(Event e)
			{
				var requestId = RequestId.Get(e);

				if(!requestId.IsAssigned)
				{
					requestId = Id.FromGuid();

					RequestId.Set(e, requestId);
				}

				return requestId;
			}

			public static void ForwardRequestId(Event source, Event target)
			{
				Flow.Traits.RequestId.Set(target, Flow.Traits.RequestId.Get(source));
			}
		}
	}
}