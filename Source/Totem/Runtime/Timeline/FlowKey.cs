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

    public override string ToString() => Text.Of(Type).WriteIf(Id.IsAssigned, $"/{Id}");

		public Flow New() => Type.New(this);

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as FlowKey);
		}

		public bool Equals(FlowKey other)
		{
			return Eq.Values(this, other).Check(x => x.Type).Check(x => x.Id);
		}

		public override int GetHashCode()
		{
      return HashCode.Combine(Type, Id);
		}

		public int CompareTo(FlowKey other)
		{
      return Cmp.Values(this, other).Check(x => x.Type).Check(x => x.Id);
    }

		public static bool operator ==(FlowKey x, FlowKey y) => Eq.Op(x, y);
		public static bool operator !=(FlowKey x, FlowKey y) => Eq.OpNot(x, y);
		public static bool operator >(FlowKey x, FlowKey y) => Cmp.Op(x, y) > 0;
		public static bool operator <(FlowKey x, FlowKey y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(FlowKey x, FlowKey y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(FlowKey x, FlowKey y) => Cmp.Op(x, y) <= 0;
	}
}