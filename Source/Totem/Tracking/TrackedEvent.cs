using System;

namespace Totem.Tracking
{
	public class TrackedEvent
	{
		protected TrackedEvent() { }

		public TrackedEvent(string eventType, long position, Id userId, DateTime eventWhen)
		{
			EventType = eventType;
			EventPosition = position;
			UserId = userId;
			EventWhen = eventWhen;
		}

		public string EventType;
		public long EventPosition;
		public Id UserId;
		public DateTime EventWhen;
	}
}
