using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem
{
	/// <summary>
	/// A reference to a view in a persistent set
	/// </summary>
	[TypeConverter(typeof(ViewKey.Converter))]
	public struct ViewKey : IEquatable<ViewKey>, IComparable<ViewKey>
	{
		private readonly string _value;

		private ViewKey(string value)
		{
			_value = value;
		}

		public bool IsByType { get { return String.IsNullOrEmpty(_value); } }

		public override string ToString()
		{
			return _value ?? "";
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is ViewKey && Equals((ViewKey) obj);
		}

		public bool Equals(ViewKey other)
		{
			return Equality.Check(this, other).Check(x => x.ToString());
		}

		public override int GetHashCode()
		{
			return IsByType ? 0 : _value.GetHashCode();
		}

		public int CompareTo(ViewKey other)
		{
			return Equality.Compare(this, other).Check(x => x.ToString());
		}

		//
		// Operators
		//

		public static bool operator ==(ViewKey x, ViewKey y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(ViewKey x, ViewKey y)
		{
			return !(x == y);
		}

		public static bool operator >(ViewKey x, ViewKey y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(ViewKey x, ViewKey y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(ViewKey x, ViewKey y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(ViewKey x, ViewKey y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Factory
		//

		public static readonly ViewKey ByType = new ViewKey();

		public static ViewKey From(string value)
		{
			return new ViewKey(value);
		}

		public static implicit operator ViewKey(string value)
		{
			return From(value);
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