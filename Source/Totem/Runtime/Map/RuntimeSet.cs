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
	public abstract class RuntimeSet<TKey, TValue> : RuntimeSetBase<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _set = new Dictionary<TKey, TValue>();

		public override int Count => _set.Count;
		public override TValue this[TKey key] => _set[key];
		public override IEnumerable<TKey> Keys => _set.Keys;
		public override IEnumerable<TValue> Values => _set.Values;
		public override IEnumerable<KeyValuePair<TKey, TValue>> Pairs => _set;

		public override bool Contains(TKey key)
		{
			return _set.ContainsKey(key);
		}

		public override TValue Get(TKey key, bool strict = true)
		{
			TValue value;

			if(!_set.TryGetValue(key, out value) && strict)
			{
				throw new KeyNotFoundException("Unknown key: " + Text.Of(key));
			}

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