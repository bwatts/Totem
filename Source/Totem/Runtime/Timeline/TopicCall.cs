using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a When method of a <see cref="Topic"/>
	/// </summary>
	public sealed class TopicCall : FlowCall
	{
		private Many<Event> _newEvents;

		public TopicCall(
			TopicType type,
			Topic instance,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
			: base(type, instance, point, dependencies, principal, cancellationToken)
		{
			_newEvents = new Many<Event>();
		}

		public void Append(Event e)
		{
			ExpectNotRetrieved();

			Flow.Traits.ForwardRequestId(Point.Event, e);

			_newEvents.Write.Add(e);
		}

		public Many<Event> RetrieveNewEvents()
		{
			ExpectNotRetrieved();

			var appendedEvents = _newEvents;

			_newEvents = null;

			return appendedEvents;
		}

		private void ExpectNotRetrieved()
		{
			Expect(_newEvents).IsNotNull("New events already retrieved");
		}
	}
}