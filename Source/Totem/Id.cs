using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem
{
	/// <summary>
	/// Identifies a persistent object by a string. May be assigned or unassigned.
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public struct Id : IWritable, IEquatable<Id>, IComparable<Id>
	{
		private readonly string _value;

		private Id(string value)
		{
			_value = value;
		}

		public bool IsUnassigned => string.IsNullOrEmpty(_value);
		public bool IsAssigned => !string.IsNullOrEmpty(_value);

    public override string ToString() => _value ?? "";
		public Text ToText() => ToString();

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is Id && Equals((Id) obj);
		}

		public bool Equals(Id other)
		{
			return Eq.Values(this, other).Check(x => x.ToString());
		}

		public override int GetHashCode()
		{
			return IsUnassigned ? 0 : _value.GetHashCode();
		}

		public int CompareTo(Id other)
		{
			return Cmp.Values(this, other).Check(x => x.ToString());
		}

		public static bool operator ==(Id x, Id y) => Eq.Op(x, y);
		public static bool operator !=(Id x, Id y) => Eq.OpNot(x, y);
		public static bool operator >(Id x, Id y) => Cmp.Op(x, y) > 0;
		public static bool operator <(Id x, Id y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(Id x, Id y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(Id x, Id y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		public static readonly Id Unassigned = new Id();

		public static Id From(string value)
		{
			return new Id((value ?? "").Trim());
		}

    public static Id From<T>(T value)
    {
      return From(value?.ToString());
    }

    public static Id FromGuid()
		{
			return new Id(Guid.NewGuid().ToString());
		}

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}