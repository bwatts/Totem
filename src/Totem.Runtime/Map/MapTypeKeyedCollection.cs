using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Totem.Map;

public class MapTypeKeyedCollection<T> : IReadOnlyCollection<T> where T : IMapTypeKeyed
{
    readonly Dictionary<MapType, T> _itemsByKey = new();

    internal void Add(T item) =>
        _itemsByKey.Add(item.MapTypeKey, item);

    public int Count => _itemsByKey.Count;
    public T this[MapType key] => Get(key);
    public IEnumerable<MapType> TypeKeys => _itemsByKey.Keys;

    public IEnumerator<T> GetEnumerator() => _itemsByKey.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(MapType key) =>
        _itemsByKey.ContainsKey(key);

    public bool TryGet(MapType key, [NotNullWhen(true)] out T? item) =>
        _itemsByKey.TryGetValue(key, out item);

    public T Get(MapType key)
    {
        if(!TryGet(key, out var type))
            throw new KeyNotFoundException($"Expected collection to contain {key}");

        return type;
    }
}
