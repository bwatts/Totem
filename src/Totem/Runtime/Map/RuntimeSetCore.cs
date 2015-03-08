using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime objects with no further differentiation, indexed by key
	/// </summary>
	/// <typeparam name="TKey">The type of key identifying runtime objects in the set</typeparam>
	/// <typeparam name="TValue">The type of keyed runtime objects in the set</typeparam>
	public abstract class RuntimeSetCore<TKey, TValue> : RuntimeSet<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _set = new Dictionary<TKey, TValue>();

		public override int Count { get { return _set.Count; } }
		public override TValue this[TKey key] { get { return _set[key]; } }
		public override IEnumerable<TKey> Keys { get { return _set.Keys; } }
		public override IEnumerable<TValue> Values { get { return _set.Values; } }
		public override IEnumerable<KeyValuePair<TKey, TValue>> Pairs { get { return _set; } }

		public override bool Contains(TKey key)
		{
			return _set.ContainsKey(key);
		}

		public override TValue Get(TKey key, bool strict = true)
		{
			TValue value;

			Expect(_set.TryGetValue(key, out value) && strict).IsFalse("Unknown key: " + Text.Of(key));

			return value;
		}

		internal void RegisterIfNotAlready(TValue value)
		{
			var key = GetKey(value);

			if(!Contains(key))
			{
				Register(key, value);
			}
		}

		internal void Register(TValue value)
		{
			Register(GetKey(value), value);
		}

		private void Register(TKey key, TValue value)
		{
			RegisterByKey(key, value);

			RegisterByOtherKeys(value);
		}

		private void RegisterByKey(TKey key, TValue value)
		{
			if(Contains(key))
			{
				throw new Exception(Text.None
					.WriteLine("The key {0} is associated with multiple values", key)
					.WriteLine()
					.WriteLine(_set[key])
					.Write(value));
			}

			_set.Add(key, value);
		}

		internal abstract TKey GetKey(TValue value);

		internal virtual void RegisterByOtherKeys(TValue value)
		{}
	}
}