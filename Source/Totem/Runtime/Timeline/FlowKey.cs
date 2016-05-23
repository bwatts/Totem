using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// References a flow instance by type and optional identifier
	/// </summary>
	public sealed class FlowKey : Notion, IEquatable<FlowKey>, IComparable<FlowKey>
	{
    private FlowKey(FlowType type, Id id)
    {
      Type = type;
      Id = id;
    }

		public readonly FlowType Type;
    public readonly Id Id;

		public override Text ToText() => Text.Of(Type).WriteIf(Id.IsAssigned, $"{Id.Separator}{Id}");

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

		public static FlowKey From(FlowType type, Id id)
		{
			return new FlowKey(type, id);
		}

		public static FlowKey From(FlowType type)
		{
			return From(type, Id.Unassigned);
		}

		public static FlowKey From(string value, bool strict = true)
		{
			var idIndex = value.IndexOf(Id.Separator);

			var typePart = idIndex == -1 ? value : value.Substring(0, idIndex);
			var idPart = idIndex == -1 ? "" : value.Substring(idIndex + 1);

			var typeKey = RuntimeTypeKey.From(typePart, strict);

			if(typeKey != null)
			{
				// Things that make you go hmmmmm
				var type = Traits.ResolveRuntime().GetFlow(typeKey, strict);

				if(type != null)
				{
					return new FlowKey(type, Id.From(idPart));
				}
			}

			ExpectNot(strict, $"Failed to parse flow key: {value}");

			return null;
		}
	}
}