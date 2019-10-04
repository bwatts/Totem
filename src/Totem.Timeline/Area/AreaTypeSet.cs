using System;
using System.Collections;
using System.Collections.Generic;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of .NET types declared in a timeline area
  /// </summary>
  /// <typeparam name="T">The type of area types in the set</typeparam>
  public class AreaTypeSet<T> : IEnumerable<T> where T : AreaType
  {
    readonly Dictionary<AreaTypeName, T> _byName = new Dictionary<AreaTypeName, T>();
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
    public T this[AreaTypeName name] => Get(name);
    public IEnumerable<AreaTypeName> Names => _byName.Keys;
    public IEnumerable<T> Types => _byName.Values;
    public IEnumerable<KeyValuePair<AreaTypeName, T>> TypesByName => _byName;

    public IEnumerator<T> GetEnumerator() => Types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(AreaTypeName name) =>
      _byName.ContainsKey(name);

        public bool TryGet(AreaTypeName name, out T type)
        {
            return _byName.TryGetValue(name, out type);
        }

        public T Get(AreaTypeName name)
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