using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing and publishing to the timeline
	/// </summary>
	[Durable]
	public abstract class Flow : Connection
	{
		[Transient] protected FlowCall Call { get; private set; }
		[Transient] protected FlowType FlowType { get; private set; }
		[Transient] protected Id Id { get; private set; }
		[Transient] protected Id RuntimeId { get; private set; }
		[Transient] protected Id EnvironmentId { get; private set; }
		[Transient] protected IViewDb Views { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
		[Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected ClaimsPrincipal Principal { get; private set; }
		[Transient] protected CancellationToken CancellationToken { get; private set; }

		public void MakeCall(FlowCall call)
		{
			StartCall(call);

			try
			{
				FlowType.CallWhen(this, Event);
			}
			finally
			{
				EndCall();
			}
		}

		private void StartCall(FlowCall call)
		{
			Expect(FlowType).IsNull(Text.Of("Flow {0} is already making a call", Id));

			Call = call;
			FlowType = call.FlowType;
			Id = call.FlowId;
			RuntimeId = call.RuntimeId;
			EnvironmentId = call.EnvironmentId;
			Views = call.Views;
			Cause = call.Cause;
			Event = call.Event;
			EventType = call.EventType;
			Principal = call.Principal;
			CancellationToken = call.CancellationToken;
		}

		private void EndCall()
		{
			Call = null;
			FlowType = null;
			Id = default(Id);
			RuntimeId = default(Id);
			EnvironmentId = default(Id);
			Views = null;
			Cause = default(TimelinePosition);
			Event = null;
			EventType = null;
			Principal = null;
			CancellationToken = default(CancellationToken);
		}

		protected void ExpectMakingCall()
		{
			Expect(Call).IsNotNull(Text.Of("Flow {0} is not making a call", Id));
		}

		//
		// Done
		//

		protected void Done()
		{
			ExpectMakingCall();

			Call.Done();
		}

		//
		// Publish
		//

		protected void Publish(Event e)
		{
			ExpectMakingCall();

			Call.Publish(this, e);
		}

		protected void Publish(IEnumerable<Event> events)
		{
			ExpectMakingCall();

			Call.Publish(this, events);
		}

		protected void Publish(params Event[] events)
		{
			ExpectMakingCall();

			Call.Publish(this, events);
		}

		//
		// Schedule
		//

		protected void Schedule(DateTime whenOccurs, Event e)
		{
			ExpectMakingCall();

			Call.Schedule(this, whenOccurs, e);
		}

		protected void Schedule(DateTime whenOccurs, IEnumerable<Event> events)
		{
			ExpectMakingCall();

			Call.Schedule(this, whenOccurs, events);
		}

		protected void Schedule(DateTime whenOccurs, params Event[] events)
		{
			Schedule(whenOccurs, events as IEnumerable<Event>);
		}

		protected void Schedule(TimeSpan whenFromNow, Event e)
		{
			Schedule(Clock.Now + whenFromNow, e);
		}

		protected void Schedule(TimeSpan whenFromNow, IEnumerable<Event> events)
		{
			Schedule(Clock.Now + whenFromNow, events);
		}

		protected void Schedule(TimeSpan whenFromNow, params Event[] events)
		{
			Schedule(whenFromNow, events as IEnumerable<Event>);
		}

		public new static class Traits
		{
			public static readonly Tag<Id> FlowId = Tag.Declare(() => FlowId);
		}
	}
}