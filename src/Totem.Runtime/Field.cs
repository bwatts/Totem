using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Reflection;

namespace Totem.Runtime
{
  /// <summary>
  /// A value bound to instances of <see cref="IBindable"/>
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

    public override string ToString() =>
      Name;

    public abstract object ResolveDefault(IBindable binding);

    public object Get(IBindable binding) =>
      binding.Fields.Get(this);

    public bool TryGet(IBindable binding, out object value) =>
      binding.Fields.TryGet(this, out value);

    public void Set(IBindable binding, object value) =>
      binding.Fields.Set(this, value);

    public void Clear(IBindable binding) =>
      binding.Fields.Clear(this);

    public void Bind(IBindable source, IBindable target) =>
      Set(target, Get(source));

    //
    // Declarations
    //

    public static Field<T> Declare<T>(Expression<Func<Field<T>>> declaration, Func<IBindable, T> resolveDefaultValue) =>
      new Field<T>(declaration.GetField(), resolveDefaultValue);

    public static Field<T> Declare<T>(Expression<Func<Field<T>>> declaration, T defaultValue = default(T)) =>
      new Field<T>(declaration.GetField(), _ => defaultValue);

    public static Field<IReadOnlyList<T>> Declare<T>(Expression<Func<Field<IReadOnlyList<T>>>> declaration) =>
      Declare(declaration, _ => new List<T>());

    public static Field<T[]> Declare<T>(Expression<Func<Field<T[]>>> declaration) =>
      Declare(declaration, _ => new T[0]);

    public static Field<List<T>> Declare<T>(Expression<Func<Field<List<T>>>> declaration) =>
      Declare(declaration, _ => new List<T>());

    public static Field<Many<T>> Declare<T>(Expression<Func<Field<Many<T>>>> declaration) =>
      Declare(declaration, _ => new Many<T>());

    public static Field<IReadOnlyDictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Field<IReadOnlyDictionary<TKey, TValue>>>> declaration) =>
      Declare(declaration, _ => new Dictionary<TKey, TValue>());

    public static Field<Dictionary<TKey, TValue>> Declare<TKey, TValue>(Expression<Func<Field<Dictionary<TKey, TValue>>>> declaration) =>
      Declare(declaration, _ => new Dictionary<TKey, TValue>());

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
  /// A value of the specified type bound to instances of <see cref="IBindable"/>
  /// </summary>
  /// <typeparam name="T">The type of bound value</typeparam>
  public class Field<T> : Field
  {
    Func<IBindable, T> _resolveDefault;

    internal Field(FieldInfo field, Func<IBindable, T> resolveDefault) : base(field, typeof(T))
    {
      _resolveDefault = resolveDefault;
    }

    public override object ResolveDefault(IBindable binding) =>
      _resolveDefault(binding);

    public T ResolveDefaultTyped(IBindable binding) =>
      _resolveDefault(binding);

    public new T Get(IBindable target) =>
      target.Fields.Get(this);

    public bool TryGet(IBindable binding, out T value) =>
      binding.Fields.TryGet(this, out value);

    public void Set(IBindable target, T value) =>
      target.Fields.Set(this, value);

    public void SetDefault(Func<IBindable, T> resolve) =>
      _resolveDefault = resolve;

    public void SetDefault(T value) =>
      _resolveDefault = _ => value;
  }
}