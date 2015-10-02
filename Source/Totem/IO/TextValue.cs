using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A text value awaiting conversion
	/// </summary>
	public class TextValue
	{
		private readonly string _value;

		public TextValue(ITypeDescriptorContext context, CultureInfo culture, string value)
		{
			Context = context;
			Culture = culture;
			_value = value;
		}

		public ITypeDescriptorContext Context { get; private set; }
		public CultureInfo Culture { get; private set; }

		public override string ToString()
		{
			return _value;
		}

		public static implicit operator string(TextValue value)
		{
			return value.ToString();
		}
	}
}