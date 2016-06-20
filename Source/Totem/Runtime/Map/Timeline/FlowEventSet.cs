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
		private readonly Dictionary<RuntimeTypeKey, FlowEvent> _eventsByKey = new Dictionary<RuntimeTypeKey, FlowEvent>();
		private readonly Dictionary<Type, FlowEvent> _eventsByDeclaredType = new Dictionary<Type, FlowEvent>();

		public IEnumerator<FlowEvent> GetEnumerator() => _eventsByKey.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public int Count => _eventsByKey.Count;

		public bool Contains(EventType e) => Contains(e.DeclaredType);
		public bool Contains(RuntimeTypeKey key) => _eventsByKey.ContainsKey(key);
		public bool Contains(Type declaredType) => _eventsByDeclaredType.ContainsKey(declaredType);
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

		public FlowEvent Get(Type declaredType, bool strict = true)
		{
			FlowEvent e;

			if(_eventsByDeclaredType.TryGetValue(declaredType, out e))
			{
				return e;
			}

			ExpectNot(strict, "Unknown event type: " + Text.Of(declaredType));

			return null;
		}

		public FlowEvent Get(Event e, bool strict = true)
		{
			return Get(e.GetType(), strict);
		}

    public IEnumerable<TimelineRoute> CallRoute(Event e)
    {
      var flowEvent = Get(e, strict: false);

      return flowEvent?.CallRoute(e) ?? new Many<TimelineRoute>();
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
			FlowEvent knownEvent;

			if(_eventsByKey.TryGetValue(e.EventType.Key, out knownEvent) && knownEvent != e)
			{
				throw new Exception(Text.Of("Event {0} is already registered", e.EventType.Key));
			}

			_eventsByKey.Add(e.EventType.Key, e);
			_eventsByDeclaredType.Add(e.EventType.DeclaredType, e);
		}
	}
}