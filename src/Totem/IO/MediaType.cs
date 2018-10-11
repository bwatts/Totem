using System;
using System.ComponentModel;
using System.Text;

namespace Totem.IO
{
  /// <summary>
  /// An identifier representing the format of a piece of media
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class MediaType : IEquatable<MediaType>, IComparable<MediaType>
  {
    MediaType(string name)
    {
      Name = name?.Split(';')[0].Trim() ?? "";
      IsNone = Name == Names.None;
    }

    public readonly string Name;
    public readonly bool IsNone;

    public override string ToString() =>
      ToString();

    public string ToString(bool escaped = false, Encoding encoding = null)
    {
      var text = escaped ? Uri.EscapeDataString(Name) : Name;

      if(encoding != null)
      {
        text += "; charset=" + encoding.WebName;
      }

      return text;
    }

    public string ToStringUtf8(bool escaped = false) =>
      ToString(escaped, Encoding.UTF8);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as MediaType);

    public bool Equals(MediaType other) =>
      !(other is null) && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
      Name.GetHashCode();

    public int CompareTo(MediaType other) =>
      other == null ? 1 : String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(MediaType x, MediaType y) => Eq.Op(x, y);
    public static bool operator !=(MediaType x, MediaType y) => Eq.OpNot(x, y);
    public static bool operator >(MediaType x, MediaType y) => Cmp.Op(x, y) > 0;
    public static bool operator <(MediaType x, MediaType y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(MediaType x, MediaType y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(MediaType x, MediaType y) => Cmp.Op(x, y) <= 0;

    public static implicit operator MediaType(string name) => new MediaType(name);

    //
    // Inherent
    //

    public static class Names
    {
      public const string None = "";
      public const string Css = "text/css";
      public const string Html = "text/html";
      public const string Javascript = "application/javascript";
      public const string Json = "application/json";
      public const string Plain = "text/plain";
      public const string Xml = "text/xml";
    }

    public static readonly MediaType None = From(Names.None);
    public static readonly MediaType Css = From(Names.Css);
    public static readonly MediaType Html = From(Names.Html);
    public static readonly MediaType Javascript = From(Names.Javascript);
    public static readonly MediaType Json = From(Names.Json);
    public static readonly MediaType Plain = From(Names.Plain);
    public static readonly MediaType Xml = From(Names.Xml);

    public static readonly Many<MediaType> AllKnown = Many.Of(Css, Html, Javascript, Json, Plain, Xml);
    public static readonly Many<MediaType> AllText = Many.Of(Css, Html, Javascript, Json, Plain, Xml);

    public static MediaType From(string name) =>
      new MediaType(name);

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        new MediaType(value);
    }
  }
}