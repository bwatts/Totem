using System;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// References a durable type by prefix and name
  /// </summary>
  public sealed class DurableTypeKey : IEquatable<DurableTypeKey>, IComparable<DurableTypeKey>
  {
    DurableTypeKey(DurablePrefix prefix, TypeName name)
    {
      Prefix = prefix;
      Name = name;
    }

    public readonly DurablePrefix Prefix;
    public readonly TypeName Name;

    public override string ToString() =>
      Prefix.IsNone ? Name.ToString() : $"{Prefix}:{Name}";

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as DurableTypeKey);

    public bool Equals(DurableTypeKey other) =>
      Eq.Values(this, other).Check(x => x.Prefix).Check(x => x.Name);

    public override int GetHashCode() =>
      HashCode.Combine(Prefix, Name);

    public int CompareTo(DurableTypeKey other) =>
      Cmp.Values(this, other).Check(x => x.Prefix).Check(x => x.Name);

    public static bool operator ==(DurableTypeKey x, DurableTypeKey y) => Eq.Op(x, y);
    public static bool operator !=(DurableTypeKey x, DurableTypeKey y) => Eq.OpNot(x, y);
    public static bool operator >(DurableTypeKey x, DurableTypeKey y) => Cmp.Op(x, y) > 0;
    public static bool operator <(DurableTypeKey x, DurableTypeKey y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(DurableTypeKey x, DurableTypeKey y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(DurableTypeKey x, DurableTypeKey y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static bool TryFrom(DurablePrefix prefix, string name, out DurableTypeKey key)
    {
      key = TypeName.TryFrom(name, out var parsedName)
        ? new DurableTypeKey(prefix, parsedName)
        : null;

      return key != null;
    }

    public static bool TryFrom(string prefix, TypeName name, out DurableTypeKey key)
    {
      key = DurablePrefix.TryFrom(prefix, out var parsedPrefix)
        ? new DurableTypeKey(parsedPrefix, name)
        : null;

      return key != null;
    }

    public static bool TryFrom(string prefix, string name, out DurableTypeKey key)
    {
      key = DurablePrefix.TryFrom(prefix, out var parsedPrefix) && TypeName.TryFrom(name, out var parsedName)
        ? new DurableTypeKey(parsedPrefix, parsedName)
        : null;

      return key != null;
    }

    public static bool TryFrom(string value, out DurableTypeKey key)
    {
      var parts = (value ?? "").Split(':');

      if(parts.Length == 1 && TypeName.TryFrom(parts[0], out var name))
      {
        key = new DurableTypeKey(DurablePrefix.None, name);
      }
      else if(parts.Length == 2
        && DurablePrefix.TryFrom(parts[0], out var prefix)
        && TypeName.TryFrom(parts[1], out name))
      {
        key = new DurableTypeKey(prefix, name);
      }
      else
      {
        key = null;
      }

      return key != null;
    }

    public static DurableTypeKey From(DurablePrefix prefix, TypeName name) =>
      new DurableTypeKey(prefix, name);

    public static DurableTypeKey From(DurablePrefix prefix, string name) =>
      From(prefix, TypeName.From(name));

    public static DurableTypeKey From(string prefix, TypeName name) =>
      From(DurablePrefix.From(prefix), name);

    public static DurableTypeKey From(string prefix, string name) =>
      From(DurablePrefix.From(prefix), TypeName.From(name));

    public static DurableTypeKey From(string value)
    {
      if(!TryFrom(value, out var key))
      {
        throw new FormatException($"Failed to parse the specified type key: {value}");
      }

      return key;
    }
  }
}