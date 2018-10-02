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

    public static IOLink From(string value, bool strict = true, bool extensionOptional = false)
    {
      var link = FileLink.From(value, strict: false, extensionOptional: extensionOptional) ?? FolderLink.From(value, strict: false) as IOLink;

      Expect.False(strict && link == null, "Value is not an I/O link");

      return link;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}