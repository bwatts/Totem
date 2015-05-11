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
		[Transient] protected FlowCall Call { get; private set; }
		[Transient] protected FlowType FlowType { get; private set; }
		[Transient] protected IDependencySource Dependencies { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
		[Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected ClaimsPrincipal Principal { get; private set; }
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
			Expect(Call).IsNull("Flow is already making a call");

			Call = call;
			FlowType = call.FlowType;
			Dependencies = call.Dependencies;
			Cause = call.Cause;
			Event = call.Event;
			EventType = call.EventType;
			Principal = call.Principal;
			CancellationToken = call.CancellationToken;
		}

		protected virtual Task MakeCall()
		{
			return FlowType.CallWhen(this, Event, Dependencies);
		}

		private void EndCall()
		{
			Call = null;
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
			Expect(Call).IsNotNull("Flow is not making a call");
		}

		protected void Then(Event e)
		{
			ExpectMakingCall();

			Call.Publish(this, e);
		}

		protected void Then(IEnumerable<Event> events)
		{
			ExpectMakingCall();

			Call.Publish(this, events);
		}

		protected void Then(params Event[] events)
		{
			ExpectMakingCall();

			Call.Publish(this, events);
		}

		protected void ThenDone()
		{
			ExpectMakingCall();

			Call.OnDone();
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