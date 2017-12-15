using System;

namespace Totem.Tracking
{
	public class TrackedEvent
	{
		protected TrackedEvent() { }

		public TrackedEvent(string eventType, long position, Id userId, DateTime eventWhen, string keyType, string key)
		{
			EventType = eventType;
			EventPosition = position;
			UserId = userId;
			EventWhen = eventWhen;
			KeyType = keyType;
			KeyValue = key;
		}

		public string EventType;
		public long EventPosition;
		public Id UserId;
		public DateTime EventWhen;
		public string KeyType;
		public string KeyValue;
	}
}
