using System;
using System.Collections;
using System.Collections.Generic;
using Totem.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of .NET types declared in a timeline area
  /// </summary>
  /// <typeparam name="T">The type of area types in the set</typeparam>
  public class AreaTypeSet<T> : IEnumerable<T> where T : AreaType
  {
    readonly Dictionary<TypeName, T> _byName = new Dictionary<TypeName, T>();
    readonly Dictionary<Type, T> _byDeclaredType = new Dictionary<Type, T>();

    internal AreaTypeSet(IEnumerable<T> types)
    {
      foreach(var type in types)
      {
        _byName[type.Name] = type;
        _byDeclaredType[type.DeclaredType] = type;
      }
    }

    public int Count => _byName.Count;
    public T this[TypeName name] => Get(name);
    public IEnumerable<TypeName> Names => _byName.Keys;
    public IEnumerable<T> Types => _byName.Values;
    public IEnumerable<KeyValuePair<TypeName, T>> TypesByName => _byName;

    public IEnumerator<T> GetEnumerator() => Types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(TypeName name) =>
      _byName.ContainsKey(name);

    public bool TryGet(TypeName name, out T type) =>
      _byName.TryGetValue(name, out type);

    public T Get(TypeName name)
    {
      if(!TryGet(name, out var type))
      {
        throw new KeyNotFoundException($"This set does not contain the specified name: {name}");
      }

      return type;
    }

    //
    // Declared types
    //

    public T this[Type declaredType] => Get(declaredType);
    public IEnumerable<Type> DeclaredTypes => _byDeclaredType.Keys;
    public IEnumerable<KeyValuePair<Type, T>> TypesByDeclaredType => _byDeclaredType;

    public bool Contains(Type declaredType) =>
      _byDeclaredType.ContainsKey(declaredType);

    public bool TryGet(Type declaredType, out T type) =>
      _byDeclaredType.TryGetValue(declaredType, out type);

    public T Get(Type declaredType)
    {
      if(!TryGet(declaredType, out var type))
      {
        throw new KeyNotFoundException($"This set does not contain the specified type: {declaredType}");
      }

      return type;
    }

    public T Get<TDeclared>() =>
      Get(typeof(TDeclared));
  }
}