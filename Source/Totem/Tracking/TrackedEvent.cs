using System;

namespace Totem.Tracking
{
  /// <summary>
  /// A timeline event tracked by an index
  /// </summary>
	public class TrackedEvent
	{
		public TrackedEvent(string eventType, long eventPosition, Id userId, DateTime eventWhen, string keyType, string keyValue)
		{
			EventType = eventType;
			EventPosition = eventPosition;
			UserId = userId;
			EventWhen = eventWhen;
			KeyType = keyType;
			KeyValue = keyValue;
		}

		public string EventType;
		public long EventPosition;
		public Id UserId;
		public DateTime EventWhen;
		public string KeyType;
		public string KeyValue;
	}
}