using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        CancellationToken cancellationToken)
				: base(point, e)
			{
        Dependencies = dependencies;
				CancellationToken = cancellationToken;
			}

			public readonly IDependencySource Dependencies;
			public readonly CancellationToken CancellationToken;

      public async Task Make(Flow flow)
      {
        using(var _ = TimelineMetrics.WhenTime.Measure(flow.ToPath()))
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
        using(var _ = TimelineMetrics.GivenTime.Measure(topic.ToPath()))
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
		}

		/// <summary>
		/// A call to a .When method of a <see cref="Topic"/>
		/// </summary>
		public sealed class TopicWhen : When
		{
			readonly ConcurrentQueue<Event> _newEvents;
			bool _retrieved;

			public TopicWhen(
        FlowPoint point,
				TopicEvent e,
        IDependencySource dependencies,
				CancellationToken cancellationToken)
				: base(point, e, dependencies, cancellationToken)
			{
				_newEvents = new ConcurrentQueue<Event>();
			}

      public void Append(Event e)
			{
				Flow.Traits.BindRequestId(Point.Event, e);
        Flow.Traits.BindUserId(Point.Event, e);

        var scheduled = e as EventScheduled;

        if(scheduled != null)
        {
          Flow.Traits.BindUserId(Point.Event, scheduled.Event);
        }

        _newEvents.Enqueue(e);
			}

			public Many<Event> RetrieveNewEvents()
			{
				Expect.False(_retrieved, "New events already retrieved");

				_retrieved = true;

				return _newEvents.ToMany();
			}
		}
  }
}