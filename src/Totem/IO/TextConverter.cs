using System;
using System.ComponentModel;
using System.Globalization;

namespace Totem.IO
{
  /// <summary>
  /// A converter to and from text representations of values
  /// </summary>
  public abstract class TextConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
      true;

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
      destinationType == typeof(string) || destinationType == typeof(Text) || base.CanConvertTo(context, destinationType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
      ConvertFrom(new TextValue(context, culture, value?.ToString() ?? ""));

    protected abstract object ConvertFrom(TextValue value);

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if(destinationType == typeof(string))
      {
        return Text.Of(value).ToString();
      }
      else if(destinationType == typeof(Text))
      {
        return Text.Of(value);
      }
      else
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
  }
}