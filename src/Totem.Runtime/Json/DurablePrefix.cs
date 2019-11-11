using System;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// An identifier distinguishing families of durable types
  /// </summary>
  public sealed class DurablePrefix : IEquatable<DurablePrefix>, IComparable<DurablePrefix>
  {
    readonly string _value;

    DurablePrefix(string value)
    {
      _value = value;
    }

    public bool IsNone => _value == "";

    public override string ToString() => _value;

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as DurablePrefix);

    public bool Equals(DurablePrefix other) =>
      Eq.Values(this, other).Check(x => x._value);

    public override int GetHashCode() =>
      _value.GetHashCode();

    public int CompareTo(DurablePrefix other) =>
      Cmp.Values(this, other).Check(x => x._value);

    public static bool operator ==(DurablePrefix x, DurablePrefix y) => Eq.Op(x, y);
    public static bool operator !=(DurablePrefix x, DurablePrefix y) => Eq.OpNot(x, y);
    public static bool operator >(DurablePrefix x, DurablePrefix y) => Cmp.Op(x, y) > 0;
    public static bool operator <(DurablePrefix x, DurablePrefix y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(DurablePrefix x, DurablePrefix y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(DurablePrefix x, DurablePrefix y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static readonly DurablePrefix None = new DurablePrefix("");

    public static bool TryFrom(string value, out DurablePrefix prefix)
    {
      if(string.IsNullOrWhiteSpace(value))
      {
        prefix = None;
      }
      else if(Id.IsName(value))
      {
        prefix = new DurablePrefix(value);
      }
      else
      {
        prefix = null;
      }

      return prefix != null;
    }

    public static DurablePrefix From(string value)
    {
      if(!TryFrom(value, out var prefix))
      {
        throw new FormatException($"Failed to parse the specified prefix: \"{value}\". Expected a name following standard identifier rules.");
      }

      return prefix;
    }
  }
}