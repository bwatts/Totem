using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Totem.IO
{
	/// <summary>
	/// A sequences of bytes
	/// </summary>
	[TypeConverter(typeof(Binary.Converter))]
	public struct Binary : IWritable, IEquatable<Binary>
	{
		public static readonly Binary None = new Binary();

		private readonly byte[] _data;

		private Binary(byte[] data) : this()
		{
			_data = data;
		}

		public int Length { get { return _data == null ? 0 : _data.Length; } }
		public long LongLength { get { return _data == null ? 0 : _data.LongLength; } }

		public override string ToString()
		{
			return ToText();
		}

		public Text ToText()
		{
			return _data == null ? Text.None : ToBase64().ToText();
		}

		public Text ToText(Encoding encoding)
		{
			var localData = _data;

			return Text.Of(() => encoding.GetString(localData));
		}

		public Text ToTextUtf8()
		{
			return ToText(Encoding.UTF8);
		}

		public byte[] ToBytes()
		{
			var copy = new byte[_data.Length];

			_data.CopyTo(copy, 0);

			return copy;
		}

		public Base64 ToBase64()
		{
			return Base64.From(this);
		}

		public Hex ToHex()
		{
			return Hex.From(this);
		}

		public MemoryStream ToStream()
		{
			return new MemoryStream(_data);
		}

		public Binary Append(Binary other)
		{
			var data = new byte[Length + other.Length];

			_data.CopyTo(data, 0);

			other._data.CopyTo(data, other.Length);

			return new Binary(data);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is Binary && Equals((Binary) obj);
		}

		public bool Equals(Binary other)
		{
			return other != null && (_data == other._data || (_data != null && _data.SequenceEqual(other._data)));
		}

		public override int GetHashCode()
		{
			return _data == null ? 0 : HashCode.CombineItems(_data);
		}

		//
		// Operators
		//

		public static bool operator ==(Binary x, Binary y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(Binary x, Binary y)
		{
			return !(x == y);
		}

		public static implicit operator Binary(byte[] data)
		{
			return From(data);
		}

		public static implicit operator byte[](Binary value)
		{
			return value.ToBytes();
		}

		public static Binary operator +(Binary x, Binary y)
		{
			return x.Append(y);
		}

		//
		// Factory
		//

		public static Binary From(IEnumerable<byte> value)
		{
			return new Binary(value.ToArray());
		}

		public static Binary From(Stream value)
		{
			var data = new MemoryStream();

			value.CopyTo(data);

			return new Binary(data.ToArray());
		}

		public static Binary From(string value, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.Default;

			return new Binary(encoding.GetBytes(value));
		}

		public static Binary FromUtf8(string value)
		{
			return From(value, Encoding.UTF8);
		}

		public static Binary Random(int byteCount)
		{
			var data = new byte[byteCount];

			new RNGCryptoServiceProvider().GetBytes(data);

			return new Binary(data);
		}

		public sealed class Converter : TypeConverter
		{
			//
			// From
			//

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string)
					|| sourceType == typeof(Text)
					|| typeof(IEnumerable<byte>).IsAssignableFrom(sourceType)
					|| base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if(value is string)
				{
					return From((string) value);
				}
				else if(value is Text)
				{
					return From((Text) value);
				}
				else if(value is IEnumerable<byte>)
				{
					return From((IEnumerable<byte>) value);
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
				}
			}

			//
			// To
			//

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string)
					|| destinationType == typeof(Text)
					|| destinationType == typeof(byte[])
					|| destinationType == typeof(IEnumerable<byte>)
					|| destinationType == typeof(IList<byte>)
					|| destinationType == typeof(List<byte>)
					|| base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if(value is Binary)
				{
					var binary = (Binary) value;

					if(destinationType == typeof(string))
					{
						return binary.ToString();
					}
					else if(destinationType == typeof(Text))
					{
						return binary.ToText();
					}
					else if(destinationType == typeof(byte[]) || destinationType == typeof(IEnumerable<byte>))
					{
						return binary.ToBytes();
					}
					else
					{
						if(destinationType == typeof(IList<byte>) || destinationType == typeof(List<byte>))
						{
							return binary.ToBytes().ToList();
						}
					}
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}