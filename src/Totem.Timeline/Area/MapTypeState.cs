using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of named values forming the runtime state of an object
  /// </summary>
  public class MapTypeState : IEnumerable<MapTypeStatePart>, IReadOnlyDictionary<string, MapTypeStatePart>
  {
    readonly Dictionary<string, MapTypeStatePart> _partsByName;

    public MapTypeState(Type type)
    {
      var parts =
        from member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        where member.MemberType.HasFlag(MemberTypes.Field | MemberTypes.Property)
        where !member.IsDefined(typeof(CompilerGeneratedAttribute))
        select new MapTypeStatePart(member);

      _partsByName = parts.ToDictionary(part => part.Name);
    }

    public int Count => _partsByName.Count;
    public bool Any => _partsByName.Count > 0;
    public IEnumerable<string> Names => _partsByName.Keys;
    public IEnumerable<MapTypeStatePart> Parts => _partsByName.Values;
    public MapTypeStatePart this[string name] => _partsByName[name];

    public IEnumerator<MapTypeStatePart> GetEnumerator() => _partsByName.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsName(string name) => _partsByName.ContainsKey(name);

    public MapTypeStatePart Get(string name, bool strict = true)
    {
      if(!ContainsName(name))
      {
        Expect.False(strict, $"Unknown part name: {name}");

        return null;
      }

      return _partsByName[name];
    }

    //
    // IReadOnlyDictionary
    //

    IEnumerable<string> IReadOnlyDictionary<string, MapTypeStatePart>.Keys => _partsByName.Keys;
    IEnumerable<MapTypeStatePart> IReadOnlyDictionary<string, MapTypeStatePart>.Values => _partsByName.Values;

    bool IReadOnlyDictionary<string, MapTypeStatePart>.ContainsKey(string key) =>
      _partsByName.ContainsKey(key);

    bool IReadOnlyDictionary<string, MapTypeStatePart>.TryGetValue(string key, out MapTypeStatePart value) =>
      _partsByName.TryGetValue(key, out value);

    IEnumerator<KeyValuePair<string, MapTypeStatePart>> IEnumerable<KeyValuePair<string, MapTypeStatePart>>.GetEnumerator() =>
      _partsByName.GetEnumerator();
  }
}