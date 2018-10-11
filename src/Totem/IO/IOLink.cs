using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A rooted reference to an IO resource
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public abstract class IOLink : LinkPart
  {
    internal IOLink()
    {}

    public sealed override string ToString() =>
      ToString();

    public abstract string ToString(bool altSlash = false);

    public static bool TryFrom(string value, out IOLink link, bool extensionOptional = false)
    {
      if(FileLink.TryFrom(value, out var file, extensionOptional))
      {
        link = file;
      }
      else if(FolderLink.TryFrom(value, out var folder))
      {
        link = folder;
      }
      else
      {
        link = null;
      }

      return link != null;
    }

    public static IOLink From(string value, bool extensionOptional = false)
    {
      if(!TryFrom(value, out var link, extensionOptional))
      {
        throw new FormatException($"Failed to parse folder or file link: {value}");
      }

      return link;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}