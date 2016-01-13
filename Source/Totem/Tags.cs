using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// A set of tags and corresponding values
	/// </summary>
	public sealed class Tags : IReadOnlyDictionary<Tag, TagValue>
	{
		private readonly Dictionary<Tag, TagValue> _pairs = new Dictionary<Tag, TagValue>();

		public int Count { get { return _pairs.Count; } }
		public IEnumerable<Tag> Keys { get { return _pairs.Keys; } }
		public IEnumerable<TagValue> Values { get { return _pairs.Values; } }
		public TagValue this[Tag key] { get { return _pairs[key]; } }

		public IEnumerator<KeyValuePair<Tag, TagValue>> GetEnumerator()
		{
			return _pairs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool IsSet(Tag tag)
		{
			return _pairs.ContainsKey(tag);
		}

		public void Set(Tag tag)
		{
			_pairs[tag] = new TagValue(tag);
		}

		public void Set<T>(Tag<T> tag, T value)
		{
			_pairs[tag] = new TagValue(tag, value);
		}

		public void Clear(Tag tag)
		{
			_pairs.Remove(tag);
		}

		public object Get(Tag tag, bool throwIfUnset = false)
		{
			TagValue value;

			if(!_pairs.TryGetValue(tag, out value))
			{
				value = new TagValue(tag);

				_pairs[tag] = value;
			}

      Expect.That(throwIfUnset && value.IsUnset).IsFalse("Get is strict and tag is not set");

			return value.Content;
		}

		public T Get<T>(Tag<T> tag, bool throwIfUnset = false)
		{
			return (T) Get((Tag) tag, throwIfUnset);
		}

		bool IReadOnlyDictionary<Tag, TagValue>.ContainsKey(Tag key)
		{
			return _pairs.ContainsKey(key);
		}

		bool IReadOnlyDictionary<Tag, TagValue>.TryGetValue(Tag key, out TagValue value)
		{
			return _pairs.TryGetValue(key, out value);
		}
	}
}