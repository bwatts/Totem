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
	/// A timeline presence observing and reacting to events
	/// </summary>
	[Durable]
	public abstract class Flow : Notion
	{
    [Transient] public FlowKey Key { get; private set; }
    [Transient] public FlowType Type { get; private set; }
    [Transient] public Id Id { get; private set; }
    [Transient] public TimelinePosition Checkpoint { get; private set; }
    [Transient] public bool Done { get; private set; }

    public override Text ToText() => Key.ToString();

    public static void Initialize(Flow flow, FlowKey key)
    {
      Expect(flow.Key).IsNull("Flow is already initialized");

      flow.Key = key;
      flow.Type = key.Type;
      flow.Id = key.Id;
      flow.Checkpoint = TimelinePosition.None;
      flow.Done = false;
    }

    public static void Initialize(Flow flow, FlowKey key, TimelinePosition checkpoint, bool done)
    {
      Expect(flow.Key).IsNull("Flow is already initialized");

      flow.Key = key;
      flow.Type = key.Type;
      flow.Id = key.Id;
      flow.Checkpoint = checkpoint;
      flow.Done = done;
    }

    //
    // Calls
    //

    [Transient] protected TimelinePosition Cause { get; private set; }
		[Transient] protected Event Event { get; private set; }
		[Transient] protected EventType EventType { get; private set; }
    [Transient] protected WhenCall WhenCall { get; private set; }
    [Transient] protected IDependencySource Dependencies { get; private set; }
		[Transient] protected ClaimsPrincipal Principal { get; private set; }
		[Transient] protected CancellationToken CancellationToken { get; private set; }

    public void MakeCall(GivenCall call)
		{
      try
      {
        StartCall(call);

        call.Make();
      }
      finally
      {
        EndCall();
      }
		}

    public async Task MakeCall(WhenCall call)
		{
			try
			{
        StartCall(call);

				await InvokeCall(call);
			}
			finally
			{
				EndCall();
			}
		}

    private void StartCall(FlowCall call)
    {
      Expect(Key).IsNotNull("Flow is not initialized");
      Expect(Event).IsNull("Flow is already making a call");

      WhenCall = call as WhenCall;
      Cause = call.Point.Cause;
      Event = call.Point.Event;
      EventType = call.Point.EventType;
    }

		protected virtual Task InvokeCall(WhenCall call)
		{
			return call.Make();
		}

    private void EndCall()
    {
      WhenCall = null;
      Cause = default(TimelinePosition);
      Event = null;
      EventType = null;
      WhenCall = null;
      Dependencies = null;
      Principal = null;
      CancellationToken = default(CancellationToken);
    }

    protected void ExpectCallingWhen()
		{
			Expect(WhenCall).IsNotNull("Flow is not calling a When method");
		}

		protected void ThenDone()
		{
			ExpectCallingWhen();

			ExpectNot(Done, "Flow is already done");

      Done = true;
		}

		public new static class Traits
		{
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
				RequestId.Set(target, RequestId.Get(source));
			}
		}
	}
}