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
	public abstract class Tag
	{
		internal Tag(FieldInfo field)
		{
			Field = field;
			Name = field.Name;
			Type = field.FieldType;
		}

		public FieldInfo Field { get; }
		public string Name { get; }
		public Type Type { get; }

		public override string ToString() => Name;

		public abstract object ResolveDefault();

		public bool IsUnset(ITaggable target)
		{
			return target.Tags.IsUnset(this);
		}

		public object Get(ITaggable target, bool throwIfUnset = false)
		{
			return target.Tags.Get(this, throwIfUnset);
		}

		public void Set(ITaggable target, object value)
		{
			target.Tags.Set(this, value);
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

		public static Tag<T[]> Declare<T>(Expression<Func<Tag<T[]>>> getField)
		{
			return new Tag<T[]>(getField.GetFieldInfo(), () => new T[0]);
		}

		public static Tag<List<T>> Declare<T>(Expression<Func<Tag<List<T>>>> getField)
		{
			return new Tag<List<T>>(getField.GetFieldInfo(), () => new List<T>());
		}

		public static Tag<Many<T>> Declare<T>(Expression<Func<Tag<Many<T>>>> getField)
		{
			return new Tag<Many<T>>(getField.GetFieldInfo(), () => new Many<T>());
		}

		public static Tag<IReadOnlyDictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Tag<IReadOnlyDictionary<TKey, TValue>>>> getField)
		{
			return new Tag<IReadOnlyDictionary<TKey, TValue>>(getField.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}

		public static Tag<Dictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Tag<Dictionary<TKey, TValue>>>> getField)
		{
			return new Tag<Dictionary<TKey, TValue>>(getField.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}

		public static IEnumerable<Tag> ReadDeclaredTags(Type type, bool nonPublic = false)
		{
			var flags = BindingFlags.Public | BindingFlags.Static;

			if(nonPublic)
			{
				flags |= BindingFlags.NonPublic;
			}

			return type
				.GetFields(flags)
				.Where(field => typeof(Tag).IsAssignableFrom(field.FieldType))
				.Select(field => (Tag) field.GetValue(null));
		}
	}

	/// <summary>
	/// Metadata of the specified type attached to taggable objects
	/// </summary>
	/// <typeparam name="T">The type of attached metadata</typeparam>
	public sealed class Tag<T> : Tag
	{
		private Func<T> _resolveDefault;

		internal Tag(FieldInfo field, Func<T> resolveDefault) : base(field)
		{
			_resolveDefault = resolveDefault;
		}

		internal Tag(FieldInfo field) : this(field, () => default(T))
		{}

		internal Tag(FieldInfo field, T defaultValue) : this(field, () => defaultValue)
		{}

		public override object ResolveDefault()
		{
			return _resolveDefault();
		}

		public new T Get(ITaggable target, bool throwIfUnset = false)
		{
			return target.Tags.Get(this, throwIfUnset);
		}

		public void Set(ITaggable target, T value)
		{
			target.Tags.Set(this, value);
		}

		public void SetDefault(Func<T> resolve)
		{
			_resolveDefault = resolve;
		}

		public void SetDefault(T value)
		{
			_resolveDefault = () => value;
		}
	}
}