using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.IO.TextConverter"/> class
	/// </summary>
	public class TextConverterScenarios : Scenarios
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
			Expect(converter.CanConvertFrom(typeof(string))).IsTrue();
		}

		void CanConvertFromText()
		{
			Expect(converter.CanConvertFrom(typeof(Text))).IsTrue();
		}

		void CanConvertFromInt32()
		{
			Expect(converter.CanConvertFrom(typeof(int))).IsTrue();
		}

		//
		// CanConvertTo
		//

		void CanConvertToString()
		{
			Expect(converter.CanConvertTo(typeof(string))).IsTrue();
		}

		void CanConvertToText()
		{
			Expect(converter.CanConvertTo(typeof(Text))).IsTrue();
		}

		void CanConvertToInt32()
		{
			Expect(converter.CanConvertTo(typeof(int))).IsFalse();
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

			Expect(value is Text).IsTrue();
			Expect(value.ToString()).Is("1");
		}

		void ConvertToInt32()
		{
			ExpectThrows<NotSupportedException>(() => converter.ConvertTo("1", typeof(int)));
		}
	}
}