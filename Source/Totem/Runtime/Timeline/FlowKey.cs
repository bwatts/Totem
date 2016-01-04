using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// References a flow instance by type and optional identifier
	/// </summary>
	public sealed class FlowKey : IEquatable<FlowKey>, IComparable<FlowKey>
	{
    public FlowKey(FlowType type, Id id)
    {
      Type = type;
      Id = id;
    }

    public readonly FlowType Type;
    public readonly Id Id;

    public override string ToString()
		{
			return Text.Of(Type).WriteIf(Id.IsAssigned, $"/{Id}");
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FlowKey);
		}

		public bool Equals(FlowKey other)
		{
			return Equality.Check(this, other).Check(x => x.Type).Check(x => x.Id);
		}

		public override int GetHashCode()
		{
      return HashCode.Combine(Type, Id);
		}

		public int CompareTo(FlowKey other)
		{
      return Equality.Compare(this, other).Check(x => x.Type).Check(x => x.Id);
    }

		public static bool operator ==(FlowKey x, FlowKey y) => Equality.CheckOp(x, y);
		public static bool operator !=(FlowKey x, FlowKey y) => !(x == y);
		public static bool operator >(FlowKey x, FlowKey y) => Equality.CompareOp(x, y) > 0;
		public static bool operator <(FlowKey x, FlowKey y) => Equality.CompareOp(x, y) < 0;
		public static bool operator >=(FlowKey x, FlowKey y) => Equality.CompareOp(x, y) >= 0;
		public static bool operator <=(FlowKey x, FlowKey y) => Equality.CompareOp(x, y) <= 0;
  }
}