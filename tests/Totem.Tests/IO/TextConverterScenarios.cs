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

		//
		// CanConvertFrom
		//

		void CanConvertFromString()
		{
			ExpectTrue(new TestTextConverter().CanConvertFrom(typeof(string)));
		}

		void CanConvertFromText()
		{
			ExpectTrue(new TestTextConverter().CanConvertFrom(typeof(Text)));
		}

		void CanConvertFromInt32()
		{
			ExpectTrue(new TestTextConverter().CanConvertFrom(typeof(int)));
		}

		//
		// CanConvertTo
		//

		void CanConvertToString()
		{
			ExpectTrue(new TestTextConverter().CanConvertTo(typeof(string)));
		}

		void CanConvertToText()
		{
			ExpectTrue(new TestTextConverter().CanConvertTo(typeof(Text)));
		}

		void CanConvertToInt32()
		{
			ExpectFalse(new TestTextConverter().CanConvertTo(typeof(int)));
		}

		//
		// ConvertTo
		//

		void ConvertToString()
		{
			var value = new TestTextConverter().ConvertTo(1, typeof(string));

			Expect(value).Is("1");
		}

		void ConvertToText()
		{
			var value = new TestTextConverter().ConvertTo(1, typeof(Text));

			ExpectTrue(value is Text);
			Expect(value.ToString()).Is("1");
		}

		void ConvertToInt32()
		{
			ExpectThrows<NotSupportedException>(() => new TestTextConverter().ConvertTo("1", typeof(int)));
		}
	}
}