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
	/// A call to a method defined by a <see cref="Timeline.Flow"/>
	/// </summary>
	public abstract class FlowCall
	{
		internal FlowCall(FlowPoint point, FlowEvent e)
		{
			Point = point;
			Event = e;
		}

		public readonly FlowPoint Point;
		public readonly FlowEvent Event;

		/// <summary>
		/// A call to a .When method of a <see cref="Flow"/>
		/// </summary>
		public class When : FlowCall
		{
			public When(
				FlowPoint point,
				FlowEvent e,
				IDependencySource dependencies,
				ClaimsPrincipal principal,
				CancellationToken cancellationToken)
				: base(point, e)
			{
				Dependencies = dependencies;
				Principal = principal;
				CancellationToken = cancellationToken;
			}

			public readonly IDependencySource Dependencies;
			public readonly ClaimsPrincipal Principal;
			public readonly CancellationToken CancellationToken;

      public async Task Make(Flow flow)
      {
        try
        {
          flow.Call = this;

          await flow.CallWhen(this);

          flow.Context.FinishCall(flow, this);
        }
        finally
        {
          flow.Call = null;
        }
      }

      internal Task MakeInternal(Flow flow)
      {
        return Event.CallWhen(flow, this);
      }
		}

		/// <summary>
		/// A call to a .Given method of a <see cref="Topic"/>
		/// </summary>
		public sealed class Given : FlowCall
		{
			public Given(FlowPoint point, TopicEvent e) : base(point, e)
			{}

			public new TopicEvent Event => (TopicEvent) base.Event;

      public void Make(Topic topic, bool loading = false)
      {
        try
        {
          topic.Call = this;

          Event.CallGiven(topic, this);

          if(!loading)
          {
            topic.Context.FinishCall(topic, this);
          }
        }
        finally
        {
          topic.Call = null;
        }
      }
		}

		/// <summary>
		/// A call to a .When method of a <see cref="Topic"/>
		/// </summary>
		public sealed class TopicWhen : When
		{
			private readonly Many<Event> _newEvents;
			private bool _retrieved;

			public TopicWhen(
        FlowPoint point,
				TopicEvent e,
				IDependencySource dependencies,
				ClaimsPrincipal principal,
				CancellationToken cancellationToken)
				: base(point, e, dependencies, principal, cancellationToken)
			{
				_newEvents = new Many<Event>();
			}

			public void Append(Event e)
			{
				Flow.Traits.ForwardRequestId(Point.Event, e);

				_newEvents.Write.Add(e);
			}

			public Many<Event> RetrieveNewEvents()
			{
				Expect.False(_retrieved, "New events already retrieved");

				_retrieved = true;

				return _newEvents;
			}
		}
	}
}