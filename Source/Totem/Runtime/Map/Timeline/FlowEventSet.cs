using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A set of events observed within a <see cref="Flow"/>, indexed by key and declared type
	/// </summary>
	public sealed class FlowEventSet : Notion, IReadOnlyCollection<FlowEvent>
	{
		private readonly Dictionary<RuntimeTypeKey, FlowEvent> _eventsByKey = new Dictionary<RuntimeTypeKey, FlowEvent>();
		private readonly Dictionary<Type, FlowEvent> _eventsByType = new Dictionary<Type, FlowEvent>();

		public IEnumerator<FlowEvent> GetEnumerator() => _eventsByKey.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => _eventsByKey.Count;

		public bool Contains(EventType e) => Contains(e.DeclaredType);
		public bool Contains(RuntimeTypeKey key) => _eventsByKey.ContainsKey(key);
		public bool Contains(Type declaredType) => _eventsByType.ContainsKey(declaredType);
		public bool Contains(Event e) => Contains(e.GetType());

		public FlowEvent Get(RuntimeTypeKey key, bool strict = true)
		{
			FlowEvent e;

			if(_eventsByKey.TryGetValue(key, out e))
			{
				return e;
			}

			ExpectNot(strict, "Unknown event key: " + Text.Of(key));

			return null;
		}

		public FlowEvent Get(Type type, bool strict = true)
		{
			FlowEvent e;

			if(_eventsByType.TryGetValue(type, out e))
			{
				return e;
			}

			ExpectNot(strict, "Unknown event type: " + Text.Of(type));

			return null;
		}

		public FlowEvent Get(EventType type, bool strict = true)
		{
			return Get(type.DeclaredType, strict);
		}

		public FlowEvent Get(Event e, bool strict = true)
		{
			return Get(e.GetType(), strict);
		}

		internal void Register(FlowEvent e)
		{
			FlowEvent current;

			if(_eventsByKey.TryGetValue(e.EventType.Key, out current) && current != e)
			{
				throw new Exception($"Event {e.EventType} is already registered");
			}

			_eventsByKey.Add(e.EventType.Key, e);
			_eventsByType.Add(e.EventType.DeclaredType, e);
		}
	}
}