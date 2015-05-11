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
		private sealed class EventLookup<TKey> : Dictionary<TKey, List<FlowEvent>> {}

		private readonly EventLookup<RuntimeTypeKey> _eventsByKey = new EventLookup<RuntimeTypeKey>();
		private readonly EventLookup<Type> _eventsByDeclaredType = new EventLookup<Type>();

		public IEnumerator<FlowEvent> GetEnumerator()
		{
			return _eventsByKey.Values.SelectMany(events => events).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return _eventsByKey.Count; }
		}

		public bool ContainsEvent(RuntimeTypeKey key)
		{
			return _eventsByKey.ContainsKey(key);
		}

		public bool ContainsEvent(Type declaredType)
		{
			return _eventsByDeclaredType.ContainsKey(declaredType);
		}

		public bool ContainsEvent(Event e)
		{
			return ContainsEvent(e.GetType());
		}

		public IEnumerable<FlowEvent> Get(RuntimeTypeKey key, bool strict = true)
		{
			if(ContainsEvent(key))
			{
				return _eventsByKey[key];
			}

			Expect(strict).IsFalse("Unknown event key: " + Text.Of(key));

			return Enumerable.Empty<FlowEvent>();
		}

		public IEnumerable<FlowEvent> Get(Type declaredType, bool strict = true)
		{
			if(ContainsEvent(declaredType))
			{
				return _eventsByDeclaredType[declaredType];
			}

			Expect(strict).IsFalse("Unknown event type: " + Text.Of(declaredType));

			return Enumerable.Empty<FlowEvent>();
		}

		public IEnumerable<FlowEvent> Get(Event e, bool strict = true)
		{
			return Get(e.GetType(), strict);
		}

		public void CallBefore(Flow flow, Event e)
		{
			foreach(var flowEvent in this)
			{
				if(flowEvent.CanCall(e))
				{
					flowEvent.CallBefore(flow, e);
				}
			}
		}

		public Task CallWhen(Flow flow, Event e, IDependencySource dependencies)
		{
			return Task.WhenAll(
				from flowEvent in this
				where flowEvent.CanCall(e)
				select flowEvent.CallWhen(flow, e, dependencies));
		}

		internal void Register(FlowEvent e)
		{
			List<FlowEvent> events;

			if(!_eventsByKey.TryGetValue(e.EventType.Key, out events))
			{
				events = new List<FlowEvent>();

				_eventsByKey.Add(e.EventType.Key, events);
				_eventsByDeclaredType.Add(e.EventType.DeclaredType, events);
			}

			events.Add(e);
		}
	}
}