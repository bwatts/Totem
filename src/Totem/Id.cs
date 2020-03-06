using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Totem.IO;

namespace Totem
{
  /// <summary>
  /// Identifies a persistent object by a string. May be assigned or unassigned.
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public struct Id : IEquatable<Id>, IComparable<Id>
  {
    readonly string _value;

    Id(string value)
    {
      _value = value;
    }

    public bool IsUnassigned =>
      string.IsNullOrEmpty(_value);

    public bool IsAssigned =>
      !string.IsNullOrEmpty(_value);

    public override string ToString() =>
      _value ?? "";

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      obj is Id && Equals((Id) obj);

    public bool Equals(Id other) =>
      Eq.Values(this, other).Check(x => x.ToString());

    public override int GetHashCode() =>
      ToString().GetHashCode();

    public int CompareTo(Id other) =>
      Cmp.Values(this, other).Check(x => x.ToString());

    public static bool operator ==(Id x, Id y) => Eq.Op(x, y);
    public static bool operator !=(Id x, Id y) => Eq.OpNot(x, y);
    public static bool operator >(Id x, Id y) => Cmp.Op(x, y) > 0;
    public static bool operator <(Id x, Id y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(Id x, Id y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(Id x, Id y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public const string Separator = "/";

    public static readonly Id Unassigned = new Id();

    public static Id From(string value) =>
      new Id((value ?? "").Trim());

    public static Id From<T>(T value) =>
      From(value?.ToString());

    public static Id FromGuid() =>
      From(Guid.NewGuid());

    public static Id FromMany(IEnumerable<string> ids) =>
      From(ids.ToTextSeparatedBy(Separator).ToString());

    public static Id FromMany(params string[] ids) =>
      FromMany(ids as IEnumerable<string>);

    public static Id FromMany(IEnumerable<Id> ids) =>
      From(ids.ToTextSeparatedBy(Separator).ToString());

    public static Id FromMany(params Id[] ids) =>
      FromMany(ids as IEnumerable<Id>);

    public static bool IsName(string value) =>
      _nameRegex.IsMatch(value ?? "");

    static readonly Regex _nameRegex = new Regex(
      "^"                 // Start
      + "[\\\\\"]*"       // Optionally match etag quotes
      + "[A-Za-z_]"       // Match the first character (no digits)
      + "[A-Za-z_0-9]*"   // Match the remaining characters
      + "[\\\\\"]*"       // Optionally match etag quotes
      + "$",              // End
      RegexOptions.Compiled);

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}