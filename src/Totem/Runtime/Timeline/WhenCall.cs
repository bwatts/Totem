using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a When method of a <see cref="Flow"/>
	/// </summary>
	public class WhenCall : Notion
	{
		public WhenCall(
			FlowType flowType,
			Flow flowInstance,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
		{
			FlowType = flowType;
			FlowInstance = flowInstance;
			Point = point;
			Dependencies = dependencies;
			Principal = principal;
			CancellationToken = cancellationToken;

			NewEvents = new Many<Event>();
		}

		public readonly FlowType FlowType;
		public readonly Flow FlowInstance;
		public readonly TimelinePoint Point;
		public readonly IDependencySource Dependencies;
		public readonly ClaimsPrincipal Principal;
		public readonly CancellationToken CancellationToken;
		public readonly Many<Event> NewEvents;

		internal void Publish(Event e)
		{
			Flow.Traits.ForwardRequestId(Point.Event, e);

			NewEvents.Write.Add(e);
		}

		internal void Publish(IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				Publish(e);
			}
		}

		internal void Publish(params Event[] events)
		{
			Publish(events as IEnumerable<Event>);
		}

		internal void Schedule(DateTime whenOccurs, Event e)
		{
			Message.Traits.When.Set(e, whenOccurs);

			Publish(new EventScheduled(e));
		}

		internal void Schedule(DateTime whenOccurs, IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				Schedule(whenOccurs, e);
			}
		}

		internal void Schedule(DateTime whenOccurs, params Event[] events)
		{
			Schedule(whenOccurs, events as IEnumerable<Event>);
		}
	}
}