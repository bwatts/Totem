using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A set of observations made by a <see cref="Flow"/>, indexed by event type
	/// </summary>
	public sealed class FlowEventSet : Notion, IReadOnlyCollection<FlowEvent>
	{
		private sealed class EventDictionary<TKey> : Dictionary<TKey, FlowEvent> {}

		private readonly EventDictionary<RuntimeTypeKey> _eventsByKey = new EventDictionary<RuntimeTypeKey>();
		private readonly EventDictionary<Type> _eventsByDeclaredType = new EventDictionary<Type>();

		public IEnumerator<FlowEvent> GetEnumerator()
		{
			return _eventsByKey.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return _eventsByKey.Count; }
		}

		public bool Contains(EventType e)
		{
			return Contains(e.DeclaredType);
		}

		public bool Contains(RuntimeTypeKey key)
		{
			return _eventsByKey.ContainsKey(key);
		}

		public bool Contains(Type declaredType)
		{
			return _eventsByDeclaredType.ContainsKey(declaredType);
		}

		public bool Contains(Event e)
		{
			return Contains(e.GetType());
		}

		public FlowEvent Get(RuntimeTypeKey key, bool strict = true)
		{
			FlowEvent e;

			if(_eventsByKey.TryGetValue(key, out e))
			{
				return e;
			}

			Expect(strict).IsFalse("Unknown event key: " + Text.Of(key));

			return null;
		}

		public FlowEvent Get(Type declaredType, bool strict = true)
		{
			FlowEvent e;

			if(_eventsByDeclaredType.TryGetValue(declaredType, out e))
			{
				return e;
			}

			Expect(strict).IsFalse("Unknown event type: " + Text.Of(declaredType));

			return null;
		}

		public FlowEvent Get(Event e, bool strict = true)
		{
			return Get(e.GetType(), strict);
		}

		public void CallGiven(Flow flow, TimelinePoint point)
		{
			var e = Get(point.EventType.Key, strict: false);

			if(e != null)
			{
				e.CallGiven(flow, point);
			}
		}

		public async Task CallWhen(Flow flow, TimelinePoint point, IDependencySource dependencies)
		{
			var e = Get(point.EventType.Key, strict: false);

			if(e != null)
			{
				await e.CallWhen(flow, point, dependencies);
			}
		}

		internal void Register(FlowEvent e)
		{
			if(_eventsByKey.ContainsKey(e.EventType.Key))
			{
				throw new Exception(Text.Of("Event {0} is already registered", e.EventType.Key));
			}

			_eventsByKey.Add(e.EventType.Key, e);
			_eventsByDeclaredType.Add(e.EventType.DeclaredType, e);
		}
	}
}