using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// A set of fields and corresponding values
	/// </summary>
	public class Fields : ITextable, IReadOnlyDictionary<Field, FieldValue>
	{
		readonly Dictionary<Field, FieldValue> _pairs = new Dictionary<Field, FieldValue>();

    public int Count => _pairs.Count;
		public IEnumerable<Field> Keys => _pairs.Keys;
    public IEnumerable<FieldValue> Values => _pairs.Values;
    public IEnumerable<KeyValuePair<Field, FieldValue>> Pairs => _pairs;

    public IEnumerable<string> KeyNames => _pairs.Keys.Select(key => key.Name);
    public IEnumerable<object> ValueContent => Values.Select(value => value.Content);

    public FieldValue this[Field field]
    {
      get { return _pairs[field]; }
      set { _pairs[field] = value; }
    }

    public object this[string field]
    {
      get { return _pairs[GetKey(field)].Content; }
      set { _pairs[GetKey(field)].Set(value); }
    }

    public override string ToString() => ToText();
    public virtual Text ToText() => Keys.ToTextSeparatedBy(" ");

    public IEnumerator<KeyValuePair<Field, FieldValue>> GetEnumerator() => _pairs.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool IReadOnlyDictionary<Field, FieldValue>.TryGetValue(Field key, out FieldValue value) => TryGet(key, out value);

    //
    // Fields
    //

    public bool ContainsKey(Field field)
    {
      return _pairs.ContainsKey(field);
    }

    public bool IsUnset(Field field)
		{
			FieldValue value;

			return !_pairs.TryGetValue(field, out value) || value.IsUnset;
		}

		public bool IsSet(Field field)
		{
			FieldValue value;

			return _pairs.TryGetValue(field, out value) && value.IsSet;
		}

		public object Get(Field field, bool throwIfUnset = false)
		{
      FieldValue value;

      if(!_pairs.TryGetValue(field, out value))
      {
        value = new FieldValue(field);

        _pairs[field] = value;
      }

			if(throwIfUnset && value.IsUnset)
			{
        throw new ArgumentException($"Field {field} is not set", nameof(field));
      }

			return value.Content;
		}

		public TValue Get<TValue>(Field<TValue> field, bool throwIfUnset = false)
		{
			return (TValue) Get((Field) field, throwIfUnset);
		}

    public bool TryGet(Field key, out FieldValue value)
    {
      return _pairs.TryGetValue(key, out value);
    }

    public bool TryGet(Field key, out object value)
    {
      FieldValue fieldValue;

      var isSet = _pairs.TryGetValue(key, out fieldValue) && fieldValue.IsSet;

      value = isSet ? fieldValue.Content : key.ResolveDefault();

      return isSet;
    }

    public bool TryGet<TValue>(Field<TValue> key, out TValue value)
    {
      object untypedValue;

      var success = TryGet(key, out untypedValue);

      value = success ? (TValue) untypedValue : key.ResolveDefaultTyped();

      return success;
    }

    public void Set(Field field, object value)
		{
			FieldValue fieldValue;

			if(_pairs.TryGetValue(field, out fieldValue))
			{
				fieldValue.Set(value);
			}
			else
			{
				_pairs[field] = new FieldValue(field, value);
			}
		}

		public void Clear(Field field)
		{
      _pairs.Remove(field);
		}

    //
    // Fields by name
    //

    public bool IsUnset(string name, bool strict = true)
    {
      var field = GetKey(name, strict);

      return field != null && IsUnset(field);
    }

    public bool IsSet(string name, bool strict = true)
    {
      var field = GetKey(name, strict);

      return field != null && IsSet(field);
    }

    public object Get(string name, bool strict = true, bool throwIfUnset = false)
    {
      var field = GetKey(name, strict);

      return field == null ? null : Get(field, throwIfUnset);
    }

    public TValue Get<TValue>(string name, bool strict = true, bool throwIfUnset = false)
    {
      var field = GetKey(name, strict);

      var typedField = field as Field<TValue>;

      if(typedField != null && strict)
      {
        throw new ArgumentException($"Field type mismatch: .{field} is not a {typeof(TValue).FullName}", nameof(name));
      }

      return field == null ? default(TValue) : Get(typedField, throwIfUnset);
    }

    public void Set(string name, object value, bool strict = true)
    {
      var field = GetKey(name, strict);

      if(field != null)
      {
        Set(field, value);
      }
    }

    public void Clear(string name, bool strict = true)
    {
      var field = GetKey(name, strict);

      if(field != null)
      {
        Clear(field);
      }
    }

    //
    // Keys
    //

    public Field GetKey(string name, bool strict = true)
    {
      var field = Keys.FirstOrDefault(key => key.Name == name);

      if(strict && field == null)
      {
        throw new ArgumentException($"Unknown field .{name}", nameof(name));
      }

      return field;
    }

    public IEnumerable<Field> GetKeys(Type type)
    {
      return Keys.Where(key => type.IsAssignableFrom(key.Type));
    }

    public IEnumerable<Field<TValue>> GetKeys<TValue>()
    {
      return Keys.OfType<Field<TValue>>();
    }

    //
    // Values
    //

    public FieldValue GetValue(string name, bool strict = true)
    {
      var value = Pairs
        .Where(pair => pair.Key.Name == name)
        .Select(pair => pair.Value)
        .FirstOrDefault();

      if(strict && value.IsUnset)
      {
        throw new ArgumentException($"Unset field .{name}", nameof(name));
      }

      return value;
    }

    public IEnumerable<FieldValue> GetValues(Type type)
    {
      return Values.Where(value => type.IsAssignableFrom(value.Field.Type));
    }

    public IEnumerable<FieldValue> GetValues<TValue>()
    {
      return
        from pair in Pairs
        where pair.Key is Field<TValue>
        select pair.Value;
    }

    //
    // Pairs
    //

    public KeyValuePair<Field, FieldValue> GetPair(string name, bool strict = true)
    {
      var found = Pairs.FirstOrDefault(pair => pair.Key.Name == name);

      if(strict && found.Key == null)
      {
        throw new ArgumentException($"Unknown field .{name}", nameof(name));
      }

      return found;
    }

    public IEnumerable<KeyValuePair<Field, FieldValue>> GetPairs(Type type)
    {
      return Pairs.Where(pair => type.IsAssignableFrom(pair.Key.Type));
    }

    public IEnumerable<KeyValuePair<Field, FieldValue>> GetPairs<TValue>()
    {
      return Pairs.Where(pair => pair.Key is Field<TValue>);
    }
  }
}