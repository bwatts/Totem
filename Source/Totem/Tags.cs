using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// A set of tags and corresponding values
	/// </summary>
	public sealed class Tags : IReadOnlyCollection<TagValue>
	{
		private readonly Dictionary<Tag, TagValue> _pairs = new Dictionary<Tag, TagValue>();

		public int Count => _pairs.Count;
		public IEnumerable<Tag> Keys => _pairs.Keys;
		public IEnumerable<TagValue> Values => _pairs.Values;
		public TagValue this[Tag key] => _pairs[key];

		public IEnumerator<TagValue> GetEnumerator() => _pairs.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool IsUnset(Tag tag)
		{
			TagValue value;

			return !_pairs.TryGetValue(tag, out value) || value.IsUnset;
		}

		public bool IsSet(Tag tag)
		{
			TagValue value;

			return _pairs.TryGetValue(tag, out value) && value.IsSet;
		}

		public object Get(Tag tag, bool throwIfUnset = false)
		{
			TagValue value;

			if(!_pairs.TryGetValue(tag, out value))
			{
				value = new TagValue(tag);

				_pairs[tag] = value;
			}

			if(throwIfUnset && value.IsUnset)
			{
				throw new InvalidOperationException($"Tag {tag} is not set");
      }

			return value.Content;
		}

		public T Get<T>(Tag<T> tag, bool throwIfUnset = false)
		{
			return (T) Get((Tag) tag, throwIfUnset);
		}

		public void Set(Tag tag, object value)
		{
			TagValue tagValue;

			if(_pairs.TryGetValue(tag, out tagValue))
			{
				tagValue.Set(value);
			}
			else
			{
				_pairs[tag] = new TagValue(tag, value);
			}
		}

		public void Clear(Tag tag)
		{
			_pairs.Remove(tag);
		}
	}
}