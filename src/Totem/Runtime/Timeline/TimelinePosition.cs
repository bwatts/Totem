using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// A distinct point on the domain timeline
	/// </summary>
	public sealed class TimelinePosition : Notion, IEquatable<TimelinePosition>, IComparable<TimelinePosition>
	{
		public static readonly TimelinePosition Start = new TimelinePosition(-1);
		public static readonly TimelinePosition External = new TimelinePosition(-2);

		private readonly long _point;

		public TimelinePosition(long point)
		{
			_point = point;
		}

		public bool IsStart { get { return _point == -1; } }
		public bool IsExternal { get { return _point == -2; } }

		public override Text ToText()
		{
			if(IsStart)
			{
				return "<start>";
			}
			else if(IsExternal)
			{
				return "<external>";
			}
			else
			{
				return Text.Of(_point);
			}
		}

		public long ToInt64()
		{
			Expect(IsStart).IsFalse("Start position has no Int64 value");
			Expect(IsExternal).IsFalse("External position has no Int64 value");

			return _point;
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as TimelinePosition);
		}

		public bool Equals(TimelinePosition other)
		{
			return Equality.Check(this, other).Check(x => x._point);
		}

		public override int GetHashCode()
		{
			return _point.GetHashCode();
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