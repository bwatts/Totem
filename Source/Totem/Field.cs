using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Reflection;

namespace Totem
{
	/// <summary>
	/// Metadata attached to instances of <see cref="IBindable"/>
	/// </summary>
	public abstract class Field
	{
		internal Field(FieldInfo info, Type type)
		{
      Info = info;
			Type = type;
    }

    public readonly FieldInfo Info;
    public readonly Type Type;

    public string Name => Info.Name;
    public string FullName => $"{Info.DeclaringType.FullName}.{Info.Name}";

    public override string ToString() => Name;

    public abstract object ResolveDefault();

		public bool IsUnset(IBindable binding)
		{
			return binding.Fields.IsUnset(this);
		}

		public bool IsSet(IBindable binding)
		{
			return binding.Fields.IsSet(this);
		}

		public object Get(IBindable binding, bool throwIfUnset = false)
		{
			return binding.Fields.Get(this, throwIfUnset);
		}

    public bool TryGet(IBindable binding, out object value)
    {
      return binding.Fields.TryGet(this, out value);
    }

		public void Set(IBindable binding, object value)
		{
      binding.Fields.Set(this, value);
		}

		public void Clear(IBindable binding)
		{
      binding.Fields.Clear(this);
		}

    //
    // Unset value
    //

    public static readonly object UnsetValue = new Unset();

    sealed class Unset
    {
      public override string ToString()
      {
        return "<unset>";
      }
    }

    //
    // Declarations
    //

    public static Field<T> Declare<T>(Expression<Func<Field<T>>> declaration, T defaultValue = default(T))
		{
			return new Field<T>(declaration.GetFieldInfo(), defaultValue);
		}

		public static Field<T> Declare<T>(Expression<Func<Field<T>>> declaration, Func<T> resolveDefaultValue)
		{
			return new Field<T>(declaration.GetFieldInfo(), () => resolveDefaultValue());
		}

		public static Field<IReadOnlyList<T>> Declare<T>(Expression<Func<Field<IReadOnlyList<T>>>> declaration)
		{
			return new Field<IReadOnlyList<T>>(declaration.GetFieldInfo(), () => new List<T>());
		}

		public static Field<T[]> Declare<T>(Expression<Func<Field<T[]>>> declaration)
		{
			return new Field<T[]>(declaration.GetFieldInfo(), () => new T[0]);
		}

		public static Field<List<T>> Declare<T>(Expression<Func<Field<List<T>>>> declaration)
		{
			return new Field<List<T>>(declaration.GetFieldInfo(), () => new List<T>());
		}

		public static Field<Many<T>> Declare<T>(Expression<Func<Field<Many<T>>>> declaration)
		{
			return new Field<Many<T>>(declaration.GetFieldInfo(), () => new Many<T>());
		}

		public static Field<IReadOnlyDictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Field<IReadOnlyDictionary<TKey, TValue>>>> declaration)
		{
			return new Field<IReadOnlyDictionary<TKey, TValue>>(declaration.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}

		public static Field<Dictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Field<Dictionary<TKey, TValue>>>> declaration)
		{
			return new Field<Dictionary<TKey, TValue>>(declaration.GetFieldInfo(), () => new Dictionary<TKey, TValue>());
		}

		public static IEnumerable<Field> ReadDeclaredFields(Type type, bool nonPublic = false)
		{
			var flags = BindingFlags.Public | BindingFlags.Static;

			if(nonPublic)
			{
				flags |= BindingFlags.NonPublic;
			}

			return type
				.GetFields(flags)
				.Where(field => typeof(Field).IsAssignableFrom(field.FieldType))
				.Select(field => (Field) field.GetValue(null));
		}
	}

  /// <summary>
  /// Metadata of the specified type attached to instances of <see cref="IBindable"/>
  /// </summary>
  /// <typeparam name="TValue">The type of attached metadata</typeparam>
  public class Field<TValue> : Field
	{
		Func<TValue> _resolveDefault;

		internal Field(FieldInfo field, Func<TValue> resolveDefault) : base(field, typeof(TValue))
		{
			_resolveDefault = resolveDefault;
		}

		internal Field(FieldInfo field) : this(field, () => default(TValue))
		{}

		internal Field(FieldInfo field, TValue defaultValue) : this(field, () => defaultValue)
		{}

    public override object ResolveDefault() => _resolveDefault();

    public TValue ResolveDefaultTyped() => _resolveDefault();

		public new TValue Get(IBindable target, bool throwIfUnset = false)
		{
			return target.Fields.Get(this, throwIfUnset);
		}

    public bool TryGet(IBindable binding, out TValue value)
    {
      return binding.Fields.TryGet(this, out value);
    }

    public void Set(IBindable target, TValue value)
		{
			target.Fields.Set(this, value);
		}

		public void SetDefault(Func<TValue> resolve)
		{
			_resolveDefault = resolve;
		}

		public void SetDefault(TValue value)
		{
			_resolveDefault = () => value;
		}

    public void Bind(IBindable source, IBindable target)
    {
      Set(target, Get(source));
    }
  }
}