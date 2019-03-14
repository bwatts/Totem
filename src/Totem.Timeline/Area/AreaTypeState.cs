using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of named values forming the state of a timeline type
  /// </summary>
  public class AreaTypeState : IEnumerable<AreaTypeStatePart>, IReadOnlyDictionary<string, AreaTypeStatePart>
  {
    readonly Dictionary<string, AreaTypeStatePart> _partsByName;

    internal AreaTypeState(Type declaredType)
    {
      var parts =
        from member in declaredType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        where member.MemberType.HasFlag(MemberTypes.Field | MemberTypes.Property)
        where !member.IsDefined(typeof(CompilerGeneratedAttribute))
        select new AreaTypeStatePart(member);

      _partsByName = parts.ToDictionary(part => part.Name);
    }

    public int Count => _partsByName.Count;
    public bool Any => _partsByName.Count > 0;
    public IEnumerable<string> Names => _partsByName.Keys;
    public IEnumerable<AreaTypeStatePart> Parts => _partsByName.Values;
    public AreaTypeStatePart this[string name] => _partsByName[name];

    public IEnumerator<AreaTypeStatePart> GetEnumerator() => _partsByName.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsName(string name) =>
      _partsByName.ContainsKey(name);

    public bool TryGet(string name, out AreaTypeStatePart part) =>
      _partsByName.TryGetValue(name, out part);

    //
    // IReadOnlyDictionary
    //

    IEnumerable<string> IReadOnlyDictionary<string, AreaTypeStatePart>.Keys => _partsByName.Keys;
    IEnumerable<AreaTypeStatePart> IReadOnlyDictionary<string, AreaTypeStatePart>.Values => _partsByName.Values;

    bool IReadOnlyDictionary<string, AreaTypeStatePart>.ContainsKey(string key) =>
      _partsByName.ContainsKey(key);

    bool IReadOnlyDictionary<string, AreaTypeStatePart>.TryGetValue(string key, out AreaTypeStatePart value) =>
      _partsByName.TryGetValue(key, out value);

    IEnumerator<KeyValuePair<string, AreaTypeStatePart>> IEnumerable<KeyValuePair<string, AreaTypeStatePart>>.GetEnumerator() =>
      _partsByName.GetEnumerator();
  }
}