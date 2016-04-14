using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// An event observed by a <see cref="Flow"/>
	/// </summary>
	public sealed class FlowEvent
	{
		internal FlowEvent(
			FlowType flowType,
			EventType eventType,
      FlowMethodSet<FlowGiven> given,
			FlowMethodSet<FlowWhen> when,
      FlowRoute route = null)
		{
      FlowType = flowType;
			EventType = eventType;
      Given = given;
			When = when;
      Route = route;
      HasRoute = route != null;
    }

    public readonly FlowType FlowType;
		public readonly EventType EventType;
    public readonly FlowMethodSet<FlowGiven> Given;
		public readonly FlowMethodSet<FlowWhen> When;
    public readonly FlowRoute Route;
    public readonly bool HasRoute;

		public override string ToString() => $"{EventType} => {FlowType}";

		public Many<TimelineRoute> CallRoute(TimelinePoint point)
    {
			return HasRoute ? Route.Call(point.Event) : Many.Of(TimelineRoute.SingleInstance);
    }

    public void CallGiven(Flow flow, TimelinePoint point)
		{
			foreach(var given in Given.SelectMethods(point))
			{
        given.Call(flow, point.Event);
			}
		}

		public async Task CallWhen(Flow flow, TimelinePoint point, IDependencySource dependencies)
		{
			CallGiven(flow, point);

			foreach(var when in When.SelectMethods(point))
			{
				await when.Call(flow, point.Event, dependencies);
			}
		}
	}
}