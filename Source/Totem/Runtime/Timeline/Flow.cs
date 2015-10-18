using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A process observing the timeline
	/// </summary>
	[Durable]
	public abstract class Flow : Notion
	{
		[Transient] protected FlowCall Call { get; private set; }
		[Transient] protected FlowType Type { get; private set; }
		[Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
		[Transient] protected IDependencySource Dependencies { get; private set; }
		[Transient] protected ClaimsPrincipal Principal { get; private set; }
		[Transient] protected CancellationToken CancellationToken { get; private set; }

		public TimelinePosition Checkpoint { get { return Traits.Checkpoint.Get(this); } }
		public bool Done { get { return Traits.Done.Get(this); } private set { Traits.Done.Set(this, value); } }

		public void CallGiven(FlowGiven given, TimelinePoint point)
		{
			given.Call(this, point.Event);
		}

		public async Task CallWhen(FlowCall call)
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

		private void StartWhenCall(FlowCall call)
		{
			Expect(Call).IsNull("Flow is already making a call");

			Call = call;
			Type = call.Type;
			Cause = call.Point.Cause;
			Event = call.Point.Event;
			EventType = call.Point.EventType;
			Dependencies = call.Dependencies;
			Principal = call.Principal;
			CancellationToken = call.CancellationToken;
		}

		protected virtual Task MakeWhenCall()
		{
			return Type.CallWhen(this, Call.Point, Dependencies);
		}

		private void EndWhenCall()
		{
			Call = null;
			Type = null;
			Cause = default(TimelinePosition);
			Event = null;
			EventType = null;
			Dependencies = null;
			Principal = null;
			CancellationToken = default(CancellationToken);
		}

		protected void ExpectCallingWhen()
		{
			Expect(Call).IsNotNull("Flow is not calling a When method");
		}

		protected void ThenDone()
		{
			ExpectCallingWhen();

			Expect(Done).IsFalse("Flow is already done");

			Done = true;
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