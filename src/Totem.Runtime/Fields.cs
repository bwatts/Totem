using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
  /// <summary>
  /// A set of values bound to instances of <see cref="IBindable"/>
  /// </summary>
  public class Fields : IReadOnlyDictionary<Field, object>
  {
    readonly ConcurrentDictionary<Field, object> _pairs = new ConcurrentDictionary<Field, object>();
    readonly IBindable _binding;

    public Fields(IBindable binding)
    {
      _binding = binding;
    }

    public int Count => _pairs.Count;
    public IEnumerable<Field> Keys => _pairs.Keys;
    public IEnumerable<object> Values => _pairs.Values;
    public IEnumerable<(Field, object)> Pairs => _pairs.Select(pair => (pair.Key, pair.Value));
    public IEnumerable<string> Names => _pairs.Keys.Select(field => field.Name);

    public object this[Field field]
    {
      get => _pairs[field];
      set => _pairs[field] = value;
    }

    public IEnumerator<KeyValuePair<Field, object>> GetEnumerator() =>
      _pairs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      GetEnumerator();

    bool IReadOnlyDictionary<Field, object>.ContainsKey(Field field) =>
      _pairs.ContainsKey(field);

    bool IReadOnlyDictionary<Field, object>.TryGetValue(Field key, out object value) =>
      TryGet(key, out value);

    public override string ToString() =>
      Keys.ToTextSeparatedBy(" ");

    public bool Contains(Field field) =>
      _pairs.ContainsKey(field);

    public object Get(Field field) =>
      _pairs.GetOrAdd(field, _ => field.ResolveDefault(_binding));

    public TValue Get<TValue>(Field<TValue> field) =>
      (TValue) Get((Field) field);

    public bool TryGet(Field field, out object value) =>
      _pairs.TryGetValue(field, out value);

    public bool TryGet<TValue>(Field<TValue> field, out TValue value)
    {
      var success = TryGet(field, out object untypedValue);

      value = success ? (TValue) untypedValue : default(TValue);

      return success;
    }

    public void Set(Field field, object value) =>
      _pairs[field] = value;

    public void Clear(Field field) =>
      _pairs.TryRemove(field, out var _);
  }
}