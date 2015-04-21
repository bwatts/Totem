using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A set of event-bound methods in a flow type, indexed by the event type
	/// </summary>
	public sealed class FlowMethodSet : Notion, IReadOnlyCollection<FlowMethod>
	{
		private readonly ILookup<RuntimeTypeKey, FlowMethod> _methodsByKey;
		private readonly ILookup<Type, FlowMethod> _methodsByDeclaredType;

		public FlowMethodSet(IReadOnlyList<FlowMethod> methods)
		{
			_methodsByKey = methods.ToLookup(method => method.EventType.Key);
			_methodsByDeclaredType = methods.ToLookup(method => method.EventType.DeclaredType);
		}

		public IEnumerator<FlowMethod> GetEnumerator()
		{
			return _methodsByKey.SelectMany(methods => methods).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return _methodsByKey.Count; }
		}

		public IEnumerable<EventType> EventTypes
		{
			get { return this.Select(method => method.EventType); }
		}

		public bool ContainsEvent(RuntimeTypeKey key)
		{
			return _methodsByKey.Contains(key);
		}

		public bool ContainsEvent(Type declaredType)
		{
			return _methodsByDeclaredType.Contains(declaredType);
		}

		public bool ContainsEvent(Event e)
		{
			return ContainsEvent(e.GetType());
		}

		public IEnumerable<FlowMethod> Get(RuntimeTypeKey key, bool strict = true)
		{
			if(ContainsEvent(key))
			{
				return _methodsByKey[key];
			}

			Expect(strict).IsFalse("Unknown message key: " + Text.Of(key));

			return Enumerable.Empty<FlowMethod>();
		}

		public IEnumerable<FlowMethod> Get(Type declaredType, bool strict = true)
		{
			if(ContainsEvent(declaredType))
			{
				return _methodsByDeclaredType[declaredType];
			}

			Expect(strict).IsFalse("Unknown message type: " + Text.Of(declaredType));

			return Enumerable.Empty<FlowMethod>();
		}

		public IEnumerable<FlowMethod> Get(Event e, bool strict = true)
		{
			return Get(e.GetType(), strict);
		}

		public void Call(Flow flow, Event e)
		{
			foreach(var eventMethod in this.Where(method => method.EventType.IsInstance(e)))
			{
				eventMethod.Call(flow, e);
			}
		}
	}
}