using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a Before or When method of a <see cref="Flow"/>
	/// </summary>
	public class FlowCall : Notion
	{
		private readonly List<Event> _newEvents = new List<Event>();

		public FlowCall(
			FlowType flowType,
			IDependencySource dependencies,
			Event e,
			EventType eventType,
			TimelinePosition cause,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
		{
			FlowType = flowType;
			Dependencies = dependencies;
			Event = e;
			EventType = eventType;
			Cause = cause;
			Principal = principal;
			CancellationToken = cancellationToken;
		}

		public readonly FlowType FlowType;
		public readonly IDependencySource Dependencies;
		public readonly Event Event;
		public readonly EventType EventType;
		public readonly TimelinePosition Cause;
		public readonly ClaimsPrincipal Principal;
		public readonly CancellationToken CancellationToken;

		public IReadOnlyList<Event> NewEvents { get { return _newEvents; } }
		public bool IsDone { get; private set; }

		public void Publish(Flow flow, Event e)
		{
			Flow.Traits.ForwardRequestId(Event, e);

			_newEvents.Add(e);
		}

		public void Publish(Flow flow, IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				Publish(flow, e);
			}
		}

		public void Publish(Flow flow, params Event[] events)
		{
			Publish(flow, events as IEnumerable<Event>);
		}

		public void OnDone()
		{
			Expect(IsDone).IsFalse("Flow is already done");

			IsDone = true;
		}
	}
}