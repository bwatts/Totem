﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// A binary value encoded as hexadecimal text
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class Hex : ITextable, IEquatable<Hex>, IComparable<Hex>
	{
		public static readonly Hex None = new Hex("", Binary.None);

		private readonly string _text;
		private readonly Binary _data;

		private Hex(string text, Binary data)
		{
			_text = text;
			_data = data;
		}

		public int TextLength => _text.Length;
		public int DataLength => _data.Length;
		public long DataLongLength => _data.LongLength;

		public override string ToString() => _text;
		public Text ToText() => _text;

		public Binary ToBinary() => _data;
		public byte[] ToBytes() => _data.ToBytes();

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as Hex);
		}

		public bool Equals(Hex other)
		{
			return Eq.Values(this, other).Check(x => x._text);
		}

		public override int GetHashCode()
		{
			return _text.GetHashCode();
		}

		public int CompareTo(Hex other)
		{
			return Cmp.Values(this, other).Check(x => x._text);
		}

		public static bool operator ==(Hex x, Hex y) => Eq.Op(x, y);
		public static bool operator !=(Hex x, Hex y) => Eq.OpNot(x, y);
		public static bool operator >(Hex x, Hex y) => Cmp.Op(x, y) > 0;
		public static bool operator <(Hex x, Hex y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(Hex x, Hex y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(Hex x, Hex y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		public static Hex From(string value, bool strict = true)
		{
      value = value.ToLower();

			if(value.Length % 2 != 0)
			{
				Expect.False(strict, "Value is not an even length: " + value);

				return None;
			}

			var data = new byte[value.Length / 2];

			for(var i = 0; i < value.Length; i += 2)
			{
				var hexValue = value.Substring(i, 2);

				try
				{
					data[i / 2] = Convert.ToByte(hexValue, 16);
				}
				catch(FormatException exception)
				{
					Expect.False(strict, Text
						.Of("Failed to parse value: ")
						.Write(value)
						.WriteTwoLines()
						.Write(exception));

					return None;
				}
			}

			return new Hex(value, data);
		}

		public static Hex From(Binary value)
		{
      var text = BitConverter.ToString(value.ToBytes()).Replace("-", "").ToLower();

      return new Hex(text, value);
		}

		public sealed class Converter : BinaryConverter
		{
			protected override object ConvertFromText(ITypeDescriptorContext context, CultureInfo culture, Text value)
			{
				return From(value);
			}

			protected override object ConvertFromBytes(ITypeDescriptorContext context, CultureInfo culture, IEnumerable<byte> value)
			{
				return From(Binary.From(value));
			}

			protected override Text ConvertToText(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return ((Hex) value).ToText();
			}

			protected override IEnumerable<byte> ConvertToBytes(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return ((Hex) value).ToBinary().ToBytes();
			}
		}
	}
}