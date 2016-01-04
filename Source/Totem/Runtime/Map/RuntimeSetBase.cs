using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime objects, indexed by key
	/// </summary>
	/// <typeparam name="TKey">The type of key identifying runtime objects in the set</typeparam>
	/// <typeparam name="TValue">The type of keyed runtime objects in the set</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public abstract class RuntimeSetBase<TKey, TValue> : Notion, IEnumerable<TValue>
	{
		public abstract int Count { get; }
		public abstract TValue this[TKey key] { get; }
		public abstract IEnumerable<TKey> Keys { get; }
		public abstract IEnumerable<TValue> Values { get; }
		public abstract IEnumerable<KeyValuePair<TKey, TValue>> Pairs { get; }

		public IEnumerator<TValue> GetEnumerator() => Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public abstract bool Contains(TKey key);

		public abstract TValue Get(TKey key, bool strict = true);
	}
}