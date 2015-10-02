using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A distinct point on the domain timeline
	/// </summary>
	public struct TimelinePosition : IEquatable<TimelinePosition>, IComparable<TimelinePosition>
	{
		public static readonly TimelinePosition None = new TimelinePosition();

		private readonly long? _point;

		public TimelinePosition(long? point) : this()
		{
			_point = point;
		}

		public bool IsNone { get { return _point == null; } }

		public override string ToString()
		{
			return "#" + (IsNone ? "-" : _point.ToString());
		}

		public long ToInt64()
		{
			Expect.That(IsNone).IsFalse("Position has no Int64 value");

			return _point.Value;
		}

		public long? ToInt64OrNone()
		{
			return _point;
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return obj is TimelinePosition && Equals((TimelinePosition) obj);
		}

		public bool Equals(TimelinePosition other)
		{
			return Equality.Check(this, other).Check(x => x._point);
		}

		public override int GetHashCode()
		{
			return _point == null ? 0 : _point.GetHashCode();
		}

		public static bool operator ==(TimelinePosition x, TimelinePosition y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(TimelinePosition x, TimelinePosition y)
		{
			return !(x == y);
		}

		//
		// Comparisons
		//

		public int CompareTo(TimelinePosition other)
		{
			return Equality.Compare(this, other).Check(x => x._point);
		}

		public static bool operator >(TimelinePosition x, TimelinePosition y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(TimelinePosition x, TimelinePosition y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(TimelinePosition x, TimelinePosition y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(TimelinePosition x, TimelinePosition y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}
	}
}