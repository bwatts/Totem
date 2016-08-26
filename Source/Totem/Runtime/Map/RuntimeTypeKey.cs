using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Totem.Http;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// Identifies a runtime type within the region of its declaring assembly
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class RuntimeTypeKey : Notion, IEquatable<RuntimeTypeKey>, IComparable<RuntimeTypeKey>
	{
		private RuntimeTypeKey(RuntimeRegionKey region, string name)
		{
			Region = region;
			Name = name;
		}

		public readonly RuntimeRegionKey Region;
		public readonly string Name;

		public override Text ToText() => Region.ToText() + ':' + Name;

		public HttpResource ToResource()
		{
			return Region.ToResource().Then(HttpResource.From(Name));
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as RuntimeTypeKey);
		}

		public bool Equals(RuntimeTypeKey other)
		{
			return Eq.Values(this, other).Check(x => x.Region).Check(x => x.Name);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Region, Name);
		}

		public int CompareTo(RuntimeTypeKey other)
		{
			return Cmp.Values(this, other).Check(x => x.Region).Check(x => x.Name);
		}

		public static bool operator ==(RuntimeTypeKey x, RuntimeTypeKey y) => Eq.Op(x, y);
		public static bool operator !=(RuntimeTypeKey x, RuntimeTypeKey y) => Eq.OpNot(x, y);
		public static bool operator >(RuntimeTypeKey x, RuntimeTypeKey y) => Cmp.Op(x, y) > 0;
		public static bool operator <(RuntimeTypeKey x, RuntimeTypeKey y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(RuntimeTypeKey x, RuntimeTypeKey y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(RuntimeTypeKey x, RuntimeTypeKey y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		public static RuntimeTypeKey From(string value, bool strict = true)
		{
			var match = _regex.Match(value);

			if(!match.Success)
			{
				Expect(!strict, "Failed to parse runtime type key: " + value);

				return null;
			}

			return From(GetCapture(match, "region"), GetCapture(match, "name"), strict);
		}

		public static RuntimeTypeKey From(string region, string name, bool strict = true)
		{
			var regionValue = RuntimeRegionKey.From(region, strict);

			return regionValue == null ? null : new RuntimeTypeKey(regionValue, name);
		}

		public static RuntimeTypeKey From(RuntimeRegionKey region, string name)
		{
			return new RuntimeTypeKey(region, name);
		}

		private static readonly Regex _regex = new Regex(
			// Start of string, then the context followed by ':'
			@"^(?<region>[A-Za-z_]\w*):"
			// The name, then the end of the string
			+ @"(?<name>[A-Za-z_]\w*)$",
			RegexOptions.Compiled);

		private static string GetCapture(Match match, string group)
		{
			var captures = match.Groups[group].Captures;

			return captures.Count == 0 ? "" : captures[0].Value;
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