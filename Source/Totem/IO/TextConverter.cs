using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A converter to and from text representations of values
	/// </summary>
	public abstract class TextConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || destinationType == typeof(Text) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value is string || value is Text
				? ConvertFrom(new TextValue(context, culture, value.ToString()))
				: base.ConvertFrom(context, culture, value);
		}

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