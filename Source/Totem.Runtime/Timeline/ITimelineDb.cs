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

		bool TryReadFlow(FlowRoute route, out Flow flow);

    TimelineMessage Push(TimelinePosition cause, Event e);

		TimelineMessage PushScheduled(TimelineMessage message);

		TimelineMessage PushStopped(FlowPoint point, Exception error);

    PushWhenResult PushWhen(Flow flow, FlowCall.When call);
  }
}