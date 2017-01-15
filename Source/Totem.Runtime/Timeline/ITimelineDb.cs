using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a database persisting events and flows on the timeline
	/// </summary>
	public interface ITimelineDb
	{
		Task<ResumeInfo> ReadResumeInfo();

		Task<Flow> ReadFlow(FlowRoute route, bool strict = true);

    Task<TimelineMessage> Push(TimelinePosition cause, Event e);

    Task<TimelineMessage> PushScheduled(TimelinePoint point);

    Task<TimelineMessage> PushStopped(FlowPoint point, Exception error);

    Task<PushTopicResult> PushTopic(Topic topic, FlowPoint point, IEnumerable<Event> newEvents);

    Task PushView(View view);
  }
}