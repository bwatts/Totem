using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.IO.TextConverter"/> class
	/// </summary>
	public class TextConverterSpecs : Specs
  {
		class TestTextConverter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				throw new NotImplementedException();
			}
		}

		TestTextConverter converter = new TestTextConverter();

		//
		// CanConvertFrom
		//

		void CanConvertFromString()
		{
			Expect(converter.CanConvertFrom(typeof(string)));
		}

		void CanConvertFromText()
		{
			Expect(converter.CanConvertFrom(typeof(Text)));
		}

		void CanConvertFromInt32()
		{
			Expect(converter.CanConvertFrom(typeof(int)));
		}

		//
		// CanConvertTo
		//

		void CanConvertToString()
		{
			Expect(converter.CanConvertTo(typeof(string)));
		}

		void CanConvertToText()
		{
			Expect(converter.CanConvertTo(typeof(Text)));
		}

		void CanConvertToInt32()
		{
			ExpectNot(converter.CanConvertTo(typeof(int)));
		}

		//
		// ConvertTo
		//

		void ConvertToString()
		{
			var value = converter.ConvertTo(1, typeof(string));

			Expect(value).Is("1");
		}

		void ConvertToText()
		{
			var value = converter.ConvertTo(1, typeof(Text));

			Expect(value is Text);
			Expect(value.ToString()).Is("1");
		}

		void ConvertToInt32()
		{
			ExpectThrows<NotSupportedException>(() => converter.ConvertTo("1", typeof(int)));
		}
	}
}