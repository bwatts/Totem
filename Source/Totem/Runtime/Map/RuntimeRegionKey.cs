using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Totem.Http;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// Identifies a region within a runtime, establishing a boundary for areas
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class RuntimeRegionKey : Notion, IEquatable<RuntimeRegionKey>, IComparable<RuntimeRegionKey>
	{
		private readonly string _name;

		private RuntimeRegionKey(string name)
		{
			_name = name;
		}

		public override Text ToText() => _name;

		public HttpResource ToResource() => HttpResource.From(_name);

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as RuntimeRegionKey);
		}

		public bool Equals(RuntimeRegionKey other)
		{
			return Eq.Values(this, other).Check(x => x._name);
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}

		public int CompareTo(RuntimeRegionKey other)
		{
			return Cmp.Values(this, other).Check(x => x._name);
		}

		public static bool operator ==(RuntimeRegionKey x, RuntimeRegionKey y) => Eq.Op(x, y);
		public static bool operator !=(RuntimeRegionKey x, RuntimeRegionKey y) => Eq.OpNot(x, y);
		public static bool operator >(RuntimeRegionKey x, RuntimeRegionKey y) => Cmp.Op(x, y) > 0;
		public static bool operator <(RuntimeRegionKey x, RuntimeRegionKey y) => Cmp.Op(x, y) < 0;
		public static bool operator >=(RuntimeRegionKey x, RuntimeRegionKey y) => Cmp.Op(x, y) >= 0;
		public static bool operator <=(RuntimeRegionKey x, RuntimeRegionKey y) => Cmp.Op(x, y) <= 0;

		//
		// Factory
		//

		// Start of string, an identifier with C# rules, end of string
		private static readonly Regex _regex = new Regex(@"^[A-Za-z_]\w*$", RegexOptions.Compiled);

		public static RuntimeRegionKey From(string value, bool strict = true)
		{
			if(!_regex.IsMatch(value))
			{
				ExpectNot(strict, "Failed to parse region key: " + value);

				return null;
			}

			return new RuntimeRegionKey(value);
		}

		public static RuntimeRegionKey From(Assembly assembly, bool strict = true)
		{
			var name = assembly.GetName().Name;

			if(name == "Totem" || name == "Totem.Runtime")
			{
				return new RuntimeRegionKey("runtime");
			}

			var attribute = assembly.GetCustomAttribute<RuntimePackageAttribute>();

			var regionKey = attribute == null ? null : attribute.Region;

			ExpectNot(strict && regionKey == null, Text
				.Of("Assembly is not a runtime package: ")
				.WriteTwoLines()
				.Write(assembly.FullName)
				.WriteTwoLines()
				.Write("Add to AssemblyInfo.cs: ")
				.WriteTwoLines()
				.Write("using {0};", typeof(RuntimePackageAttribute).Namespace)
				.WriteTwoLines()
				.Write("[assembly: RuntimePackage(region: \"...\")]"));

			return regionKey;
		}

		public static RuntimeRegionKey From(Type type, bool strict = true)
		{
			return From(type.Assembly, strict);
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