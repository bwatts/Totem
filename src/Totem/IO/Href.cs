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

    public static Href From(string value, bool strict = true)
    {
      var href = HttpLink.From(value, strict: false) ?? HttpResource.From(value, strict: false) as Href;

      Expect.False(strict && href == null, "Failed to parse href: " + value);

      return href;
    }

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}