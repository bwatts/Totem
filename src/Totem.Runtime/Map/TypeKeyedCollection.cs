using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Totem.Map;

public class TypeKeyedCollection<T> : IReadOnlyCollection<T> where T : ITypeKeyed
{
    readonly Dictionary<Type, T> _itemsByKey = new();

    internal void Add(T item) =>
        _itemsByKey.Add(item.TypeKey, item);

    public int Count => _itemsByKey.Count;
    public T this[Type key] => Get(key);
    public IEnumerable<Type> TypeKeys => _itemsByKey.Keys;

    public IEnumerator<T> GetEnumerator() => _itemsByKey.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(Type key) =>
        _itemsByKey.ContainsKey(key);

    public bool TryGet(Type key, [NotNullWhen(true)] out T? item) =>
        _itemsByKey.TryGetValue(key, out item);

    public T Get(Type key)
    {
        if(!TryGet(key, out var type))
            throw new KeyNotFoundException($"Expected collection to contain {key}");

        return type;
    }
}
