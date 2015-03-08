using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// Identifies a region within a runtime, establishing a boundary for areas
	/// </summary>
	[TypeConverter(typeof(RuntimeRegionKey.Converter))]
	public sealed class RuntimeRegionKey : Notion, IEquatable<RuntimeRegionKey>, IComparable<RuntimeRegionKey>
	{
		private RuntimeRegionKey(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public override Text ToText()
		{
			return Name;
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as RuntimeRegionKey);
		}

		public bool Equals(RuntimeRegionKey other)
		{
			return Equality.Check(this, other).Check(x => x.Name);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public int CompareTo(RuntimeRegionKey other)
		{
			return Equality.Compare(this, other).Check(x => x.Name);
		}

		//
		// Operators
		//

		public static bool operator ==(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return !(x == y);
		}

		public static bool operator >(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(RuntimeRegionKey x, RuntimeRegionKey y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Factory
		//

		// Start of string, a identifier with C# rules, end of string
		private static readonly Regex _regex = new Regex(@"^[A-Za-z_]\w*$", RegexOptions.Compiled);

		public static RuntimeRegionKey From(string value, bool strict = true)
		{
			if(!_regex.IsMatch(value))
			{
				Expect(strict).IsFalse("Failed to parse region key: " + value);

				return null;
			}

			return new RuntimeRegionKey(value);
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