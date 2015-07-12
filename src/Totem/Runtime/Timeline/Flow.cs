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
		[Transient] private FlowCall _call;

		[Transient] protected ClaimsPrincipal Principal { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
		[Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected FlowType FlowType { get; private set; }
		[Transient] protected IDependencySource Dependencies { get; private set; }
		[Transient] protected CancellationToken CancellationToken { get; private set; }

		public async Task MakeCall(FlowCall call)
		{
			StartCall(call);

			try
			{
				await MakeCall();
			}
			finally
			{
				EndCall();
			}
		}

		private void StartCall(FlowCall call)
		{
			Expect(_call).IsNull("Flow is already making a call");

			_call = call;
			Principal = call.Principal;
			Event = call.Point.Event;
			EventType = call.Point.EventType;
			Cause = call.Point.Cause;
			FlowType = call.FlowType;
			Dependencies = call.Dependencies;
			CancellationToken = call.CancellationToken;
		}

		protected virtual Task MakeCall()
		{
			return FlowType.CallWhen(this, _call.Point, Dependencies);
		}

		private void EndCall()
		{
			_call = null;
			FlowType = null;
			Dependencies = null;
			Cause = default(TimelinePosition);
			Event = null;
			EventType = null;
			Principal = null;
			CancellationToken = default(CancellationToken);
		}

		protected void ExpectMakingCall()
		{
			Expect(_call).IsNotNull("Flow is not making a call");
		}

		protected void ThenDone()
		{
			ExpectMakingCall();

			_call.OnDone();
		}

		protected void Then(Event e)
		{
			ExpectMakingCall();

			_call.Publish(this, e);
		}

		protected void Then(IEnumerable<Event> events)
		{
			ExpectMakingCall();

			_call.Publish(this, events);
		}

		protected void Then(params Event[] events)
		{
			ExpectMakingCall();

			_call.Publish(this, events);
		}

		protected void ThenAt(DateTime whenOccurs, Event e)
		{
			ExpectMakingCall();

			_call.Schedule(this, whenOccurs, e);
		}

		protected void ThenAt(DateTime whenOccurs, IEnumerable<Event> events)
		{
			ExpectMakingCall();

			_call.Schedule(this, whenOccurs, events);
		}

		protected void ThenAt(DateTime whenOccurs, params Event[] events)
		{
			ExpectMakingCall();

			_call.Schedule(this, whenOccurs, events);
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
			public static readonly Tag<Id> RequestId = Tag.Declare(() => RequestId);

			public static void ForwardRequestId(Event source, Event target)
			{
				Flow.Traits.RequestId.Set(target, Flow.Traits.RequestId.Get(source));
			}
		}
	}
}