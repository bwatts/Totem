using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.Http;
using Totem.IO;

namespace Totem
{
	/// <summary>
	/// Identifies a persistent object by a string
	/// </summary>
	[TypeConverter(typeof(Id.Converter))]
	public struct Id : IEquatable<Id>, IComparable<Id>
	{
		private readonly string _value;

		private Id(string value)
		{
			_value = value;
		}

		public bool IsUnassigned { get { return String.IsNullOrEmpty(_value); } }

		public override string ToString()
		{
			return _value ?? "";
		}

		public HttpResource ToResource()
		{
			return IsUnassigned ? HttpResource.Root : HttpResource.From(_value);
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is Id && Equals((Id) obj);
		}

		public bool Equals(Id other)
		{
			return Equality.Check(this, other).Check(x => x.ToString());
		}

		public override int GetHashCode()
		{
			return IsUnassigned ? 0 : _value.GetHashCode();
		}

		public int CompareTo(Id other)
		{
			return Equality.Compare(this, other).Check(x => x.ToString());
		}

		//
		// Operators
		//

		public static bool operator ==(Id x, Id y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(Id x, Id y)
		{
			return !(x == y);
		}

		public static bool operator >(Id x, Id y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(Id x, Id y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(Id x, Id y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(Id x, Id y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Factory
		//

		public static readonly Id Unassigned = new Id();

		public static Id From(string value)
		{
			return new Id(value);
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