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
		public FlowCall(
			ClaimsPrincipal principal,
			TimelinePoint point,
			FlowType flowType,
			IDependencySource dependencies,
			CancellationToken cancellationToken)
		{
			Principal = principal;
			Point = point;
			FlowType = flowType;
			Dependencies = dependencies;
			CancellationToken = cancellationToken;

			NewEvents = new Many<Event>();
		}

		public readonly ClaimsPrincipal Principal;
		public readonly TimelinePoint Point;
		public readonly FlowType FlowType;
		public readonly IDependencySource Dependencies;
		public readonly CancellationToken CancellationToken;
		public readonly Many<Event> NewEvents;
		public bool IsDone { get; private set; }

		public void OnDone()
		{
			Expect(IsDone).IsFalse("Flow is already done");

			IsDone = true;
		}

		public void Publish(Flow flow, Event e)
		{
			Flow.Traits.ForwardRequestId(Point.Event, e);

			NewEvents.Write.Add(e);
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

		public void Schedule(Flow flow, DateTime whenOccurs, Event e)
		{
			Message.Traits.When.Set(e, whenOccurs);

			Publish(flow, new EventScheduled(e));
		}

		public void Schedule(Flow flow, DateTime whenOccurs, IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				Schedule(flow, whenOccurs, e);
			}
		}

		public void Schedule(Flow flow, DateTime whenOccurs, params Event[] events)
		{
			Schedule(flow, whenOccurs, events as IEnumerable<Event>);
		}
	}
}