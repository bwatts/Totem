using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// An absolute or relative reference to an HTTP resource
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public abstract class Href : LinkPart
  {
    public bool IsRoot
    {
      get
      {
        var resource = this as HttpResource;

        return resource != null && resource == HttpResource.Root;
      }
    }

    public static bool TryFrom(string value, out Href href)
    {
      if(HttpLink.TryFrom(value, out var link))
      {
        href = link;
      }
      else if(HttpResource.TryFrom(value, out var resource))
      {
        href = resource;
      }
      else
      {
        href = null;
      }

      return href != null;
    }

    public static Href From(string value)
    {
      if(!TryFrom(value, out var href))
      {
        throw new FormatException($"Failed to parse href: {value}");
      }

      return href;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}