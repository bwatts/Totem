using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Reflection;

namespace Totem
{
	/// <summary>
	/// Metadata attached to taggable objects
	/// </summary>
	public class Tag : IWritable
	{
		internal Tag(FieldInfo field)
		{
			Field = field;
		}

		public FieldInfo Field { get; private set; }
		public virtual bool HasValue { get { return false; } }

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return Field.Name;
		}

		public virtual object ResolveDefaultValue()
		{
			return UnsetValue;
		}

		public bool IsSet(ITaggable target)
		{
			return target.Tags.IsSet(this);
		}

		public void Set(ITaggable target)
		{
			target.Tags.Set(this);
		}

		public object Get(ITaggable target, bool throwIfUnset = false)
		{
			return target.Tags.Get(this, throwIfUnset);
		}

		public void Clear(ITaggable target)
		{
			target.Tags.Clear(this);
		}

		//
		// Unset value
		//

		public static readonly object UnsetValue = new Unset();

		private sealed class Unset
		{
			public override string ToString()
			{
				return "<unset>";
			}
		}

		//
		// Declarations
		//

		public static Tag Declare(Expression<Func<Tag>> getField)
		{
			return new Tag(getField.GetFieldInfo());
		}

		public static Tag<T> Declare<T>(Expression<Func<Tag<T>>> getField, T defaultValue = default(T))
		{
			return new Tag<T>(getField.GetFieldInfo(), defaultValue);
		}

		public static Tag<T> Declare<T>(Expression<Func<Tag<T>>> getField, Func<T> resolveDefaultValue)
		{
			return new Tag<T>(getField.GetFieldInfo(), () => resolveDefaultValue());
		}

		public static Tag<IReadOnlyList<T>> Declare<T>(Expression<Func<Tag<IReadOnlyList<T>>>> getField)
		{
			return new Tag<IReadOnlyList<T>>(getField.GetFieldInfo(), () => new List<T>());
		}

		public static Tag<List<T>> Declare<T>(Expression<Func<Tag<List<T>>>> getField)
		{
			return new Tag<List<T>>(getField.GetFieldInfo(), () => new List<T>());
		}

		public static Tag<T[]> Declare<T>(Expression<Func<Tag<T[]>>> getField)
		{
			return new Tag<T[]>(getField.GetFieldInfo(), () => new T[0]);
		}

		public static Tag<IReadOnlyDictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Tag<IReadOnlyDictionary<TKey, TValue>>>> getField)
		{
			return new Tag<IReadOnlyDictionary<TKey, TValue>>(getField.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}

		public static Tag<Dictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Tag<Dictionary<TKey, TValue>>>> getField)
		{
			return new Tag<Dictionary<TKey, TValue>>(getField.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}
	}

	/// <summary>
	/// Metadata of the specified type attached to taggable objects
	/// </summary>
	/// <typeparam name="T">The type of attached metadata</typeparam>
	public sealed class Tag<T> : Tag
	{
		private Func<T> _resolveDefaultValue;

		internal Tag(FieldInfo field, Func<T> resolveDefaultValue) : base(field)
		{
			_resolveDefaultValue = resolveDefaultValue;
		}

		internal Tag(FieldInfo field) : this(field, () => default(T))
		{}

		internal Tag(FieldInfo field, T defaultValue) : this(field, () => defaultValue)
		{}

		public override bool HasValue { get { return true; } }

		public override object ResolveDefaultValue()
		{
			return _resolveDefaultValue();
		}

		public void Set(ITaggable target, T value)
		{
			target.Tags.Set(this, value);
		}

		public new T Get(ITaggable target, bool throwIfUnset = false)
		{
			return target.Tags.Get(this, throwIfUnset);
		}

		public void SetDefaultValue(Func<T> resolve)
		{
			_resolveDefaultValue = resolve;
		}

		public void SetDefaultValue(T value)
		{
			_resolveDefaultValue = () => value;
		}
	}
}