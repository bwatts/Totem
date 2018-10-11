using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A resource targeted by an I/O link
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public abstract class IOResource : LinkPart
  {
    internal IOResource()
    {}

    public sealed override string ToString() =>
      ToString();

    public abstract string ToString(bool altSlash = false, bool leading = false);

    public static bool TryFrom(string value, out IOResource resource)
    {
      if(FileResource.TryFrom(value, out var file))
      {
        resource = file;
      }
      else
      {
        resource = FolderResource.From(value);
      }

      return resource != null;
    }

    public static IOResource From(string value)
    {
      if(!TryFrom(value, out var resource))
      {
        throw new FormatException($"Failed to parse resource: {value}");
      }

      return resource;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}