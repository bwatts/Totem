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

    public static IOResource From(string value, bool strict = true)
    {
      var resource = FileResource.From(value, strict: false) ?? FolderResource.From(value, strict: false) as IOResource;

      Expect.False(strict && resource == null, "Value is not an I/O resource");

      return resource;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}