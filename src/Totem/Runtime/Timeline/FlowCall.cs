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
			Id flowId,
			bool isFirst,
			IDependencySource dependencies,
			IViewDb views,
			Event e,
			EventType eventType,
			TimelinePosition cause,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
		{
			FlowType = flowType;
			FlowId = flowId;
			IsFirst = isFirst;
			Dependencies = dependencies;
			Views = views;
			Event = e;
			EventType = eventType;
			Cause = cause;
			Principal = principal;
			CancellationToken = cancellationToken;
		}

		public readonly FlowType FlowType;
		public readonly Id FlowId;
		public readonly bool IsFirst;
		public readonly IDependencySource Dependencies;
		public readonly IViewDb Views;
		public readonly Event Event;
		public readonly EventType EventType;
		public readonly TimelinePosition Cause;
		public readonly ClaimsPrincipal Principal;
		public readonly CancellationToken CancellationToken;

		public IReadOnlyList<Event> NewEvents { get { return _newEvents; } }
		public bool IsDone { get; private set; }

		//
		// Publish
		//

		public void Publish(Flow flow, Event e)
		{
			Flow.Traits.FlowId.Set(e, FlowId);

			_newEvents.Add(e);
		}

		public void Publish(Flow flow, IEnumerable<Event> events)
		{
			foreach(var e in events)
			{
				Flow.Traits.FlowId.Set(e, FlowId);

				_newEvents.Add(e);
			}
		}

		public void Publish(Flow flow, params Event[] events)
		{
			Publish(flow, events as IEnumerable<Event>);
		}

		public void Done()
		{
			Expect(IsDone).IsFalse("Flow is already done");

			IsDone = true;
		}

		//
		// Schedule
		//

		public void Schedule(Flow flow, DateTime whenOccurs, Event e)
		{
			Publish(flow, new EventsScheduled(whenOccurs, Many.Of(e)));
		}

		public void Schedule(Flow flow, DateTime whenOccurs, IEnumerable<Event> events)
		{
			Publish(flow, new EventsScheduled(whenOccurs, events.ToList()));
		}

		public void Schedule(Flow flow, DateTime whenOccurs, params Event[] events)
		{
			Schedule(flow, whenOccurs, events as IEnumerable<Event>);
		}
	}
}