using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a database persisting events and flows on the timeline
	/// </summary>
	public interface ITimelineDb
	{
		ResumeInfo ReadResumeInfo();

		bool TryReadFlow(TimelineRoute route, out Flow flow);

		Many<TimelineMessage> Push(Many<Event> events);

		Many<TimelineMessage> PushCall(WhenCall call);

		TimelineMessage PushFromSchedule(TimelineMessage message);

		TimelineMessage PushFlowStopped(FlowKey key, TimelinePoint point, Exception error);
	}
}