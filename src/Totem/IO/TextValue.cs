using System.ComponentModel;
using System.Globalization;

namespace Totem.IO
{
  /// <summary>
  /// A text value awaiting conversion
  /// </summary>
  public class TextValue
  {
    readonly string _value;

    public TextValue(ITypeDescriptorContext context, CultureInfo culture, string value)
    {
      Context = context;
      Culture = culture;
      _value = value;
    }

    public readonly ITypeDescriptorContext Context;
    public readonly CultureInfo Culture;

    public override string ToString() =>
      _value;

    public static implicit operator string(TextValue value) =>
      value?.ToString() ?? "";
  }
}