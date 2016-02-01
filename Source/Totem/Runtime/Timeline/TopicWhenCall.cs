using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A call to a When method of a <see cref="Topic"/>
	/// </summary>
	public class TopicWhenCall : WhenCall
	{
		private Many<Event> _newEvents;

    internal TopicWhenCall(
			Topic topic,
			TimelinePoint point,
			IDependencySource dependencies,
			ClaimsPrincipal principal,
			CancellationToken cancellationToken)
			: base(topic, point, dependencies, principal, cancellationToken)
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
			Expect.True(_newEvents).IsNotNull("New events already retrieved");
		}
	}
}