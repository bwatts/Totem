using System;
using System.ComponentModel;
using System.IO;

namespace Totem.IO
{
  /// <summary>
  /// The extension-qualified name identifying a file
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class FileName : LinkPart, IEquatable<FileName>
  {
    FileName(LinkText text, LinkText extension)
    {
      Text = text;
      Extension = extension;
    }

    public readonly LinkText Text;
    public readonly LinkText Extension;

    public override bool IsTemplate =>
      Text.IsTemplate || Extension.IsTemplate;

    public override string ToString() =>
      Totem.Text.Of(Text).WriteIf(Extension.Length > 0, FileExtension.WithSeparator(Extension).ToString());

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as FileName);

    public bool Equals(FileName other) =>
      Eq.Values(this, other).Check(x => x.Text).Check(x => x.Extension);

    public override int GetHashCode() =>
      HashCode.Combine(Text, Extension);

    public static bool operator ==(FileName x, FileName y) => Eq.Op(x, y);
    public static bool operator !=(FileName x, FileName y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static bool TryFrom(string value, out FileName name, bool extensionOptional = false)
    {
      name = null;

      var extensionIndex = value.LastIndexOf(FileExtension.Separator);

      if(extensionIndex == -1)
      {
        if(extensionOptional)
        {
          name = new FileName(value, "");
        }
      }
      else
      {
        if(extensionIndex < value.Length - 1)
        {
          name = new FileName(
            value.Substring(0, extensionIndex),
            value.Substring(extensionIndex + 1));
        }
      }

      return name != null;
    }

    public static FileName From(string value, bool extensionOptional = false)
    {
      if(!TryFrom(value, out var name, extensionOptional))
      {
        throw new FormatException($"Failed to parse file name: {value}");
      }

      return name;
    }

    public static FileName From(LinkText text, LinkText extension) =>
      new FileName(text, extension);

    public static FileName FromRandom() =>
      From(Path.GetRandomFileName());

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}