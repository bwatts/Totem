using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// A converter to and from binary representations of values
  /// </summary>
  public abstract class BinaryConverter : TypeConverter
  {
    //
    // From
    //

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
      sourceType == typeof(string)
        || sourceType == typeof(Text)
        || typeof(IEnumerable<byte>).IsAssignableFrom(sourceType)
        || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if(value is string || value is Text)
      {
        return ConvertFromText(context, culture, value.ToString());
      }
      else if(value is IEnumerable<byte>)
      {
        return ConvertFromBytes(context, culture, (IEnumerable<byte>) value);
      }
      else
      {
        return base.ConvertFrom(context, culture, value);
      }
    }

    protected abstract object ConvertFromText(ITypeDescriptorContext context, CultureInfo culture, Text value);

    protected abstract object ConvertFromBytes(ITypeDescriptorContext context, CultureInfo culture, IEnumerable<byte> value);

    //
    // To
    //

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
      destinationType == typeof(string)
        || destinationType == typeof(Text)
        || destinationType == typeof(byte[])
        || destinationType == typeof(IEnumerable<byte>)
        || destinationType == typeof(IList<byte>)
        || destinationType == typeof(List<byte>)
        || destinationType == typeof(Many<byte>)
        || base.CanConvertTo(context, destinationType);

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if(destinationType == typeof(string))
      {
        return ConvertToText(context, culture, value).ToString();
      }
      else if(destinationType == typeof(Text))
      {
        return ConvertToText(context, culture, value);
      }
      else if(destinationType == typeof(byte[]) || destinationType == typeof(IEnumerable<byte>))
      {
        var bytes = ConvertToBytes(context, culture, value);

        return bytes as byte[] ?? bytes.ToArray();
      }
      else if(destinationType == typeof(IList<byte>) || destinationType == typeof(List<byte>))
      {
        return ConvertToBytes(context, culture, value).ToList();
      }
      else if(destinationType == typeof(Many<byte>))
      {
        return ConvertToBytes(context, culture, value).ToMany();
      }
      else
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    protected abstract Text ConvertToText(ITypeDescriptorContext context, CultureInfo culture, object value);

    protected abstract IEnumerable<byte> ConvertToBytes(ITypeDescriptorContext context, CultureInfo culture, object value);
  }
}