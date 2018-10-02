using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// The value of the HTTP Authorization header
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class HttpAuthorization : IEquatable<HttpAuthorization>, IComparable<HttpAuthorization>
  {
    HttpAuthorization(string type, string credentials)
    {
      Type = type;
      Credentials = credentials;
    }

    public string Type { get; }
    public string Credentials { get; }

    public bool HasType => Type != "";
    public bool HasCredentials => Credentials != "";

    public bool TypeIsBasic => Type.Equals("Basic", StringComparison.OrdinalIgnoreCase);
    public bool TypeIsBearer => Type.Equals("Bearer", StringComparison.OrdinalIgnoreCase);

    public override string ToString() =>
      Text.Of(Type).WriteIf(Credentials != "", " " + Credentials);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpAuthorization);

    public bool Equals(HttpAuthorization other) =>
      Eq.Values(this, other).Check(x => x.Type).Check(x => x.Credentials);

    public override int GetHashCode() =>
      HashCode.Combine(Type, Credentials);

    public int CompareTo(HttpAuthorization other) =>
      Cmp.Values(this, other).Check(x => x.Type).Check(x => x.Credentials);

    public static bool operator ==(HttpAuthorization x, HttpAuthorization y) => Eq.Op(x, y);
    public static bool operator !=(HttpAuthorization x, HttpAuthorization y) => Eq.OpNot(x, y);
    public static bool operator >(HttpAuthorization x, HttpAuthorization y) => Cmp.Op(x, y) > 0;
    public static bool operator <(HttpAuthorization x, HttpAuthorization y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(HttpAuthorization x, HttpAuthorization y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(HttpAuthorization x, HttpAuthorization y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static readonly HttpAuthorization Anonymous = new HttpAuthorization("", "");

    public static HttpAuthorization From(string type, string credentials) =>
      new HttpAuthorization(type, credentials);

    public static HttpAuthorization From(string value)
    {
      value = value.Trim();

      if(value == "")
      {
        return Anonymous;
      }

      var separatorIndex = value.IndexOf(' ');

      if(separatorIndex == -1)
      {
        return From("", value);
      }

      var type = value.Substring(0, separatorIndex);
      var credentials = value.Substring(separatorIndex + 1);

      return From(type, credentials);
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}