﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Totem.IO
{
	/// <summary>
	/// A binary value encoded as Base64 text
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class Base64 : ITextable, IEquatable<Base64>, IComparable<Base64>
	{
		public static readonly Base64 None = new Base64("", Binary.None);

		private readonly string _text;
		private readonly Binary _data;

		private Base64(string text, Binary data)
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
			return Equals(obj as Base64);
		}

		public bool Equals(Base64 other)
		{
			return Eq.Values(this, other).Check(x => x._text);
		}

		public override int GetHashCode()
		{
			return _text.GetHashCode();
		}

		public int CompareTo(Base64 other)
		{
			return Cmp.Values(this, other).Check(x => x._text);
		}

		public static bool operator ==(Base64 x, Base64 y) => Eq.Op(x, y);
		public static bool operator !=(Base64 x, Base64 y) => Eq.OpNot(x, y);
		public static bool operator >(Base64 x, Base64 y) => Cmp.Op(x, y) > 0;
		public static bool operator <(Base64 x, Base64 y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(Base64 x, Base64 y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(Base64 x, Base64 y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		public static Base64 From(string value, bool strict = true, Encoding encoding = null)
		{
			byte[] data;

			try
			{
				data = Convert.FromBase64String(value);
			}
			catch(FormatException error)
			{
				if(strict)
				{
					Expect.False(strict, Text
						.Of("Failed to parse value: ")
						.Write(value)
						.WriteTwoLines()
						.Write(error));
				}

				return null;
			}

			return new Base64(value, Binary.From(data));
		}

		public static Base64 From(Binary value)
		{
			return new Base64(Convert.ToBase64String(value.ToBytes()), value);
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
				return ((Base64) value).ToText();
			}

			protected override IEnumerable<byte> ConvertToBytes(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return ((Base64) value).ToBinary().ToBytes();
			}
		}
	}
}