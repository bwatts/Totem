using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;

namespace Totem.Http
{
	/// <summary>
	/// The DNS name of an HTTP host
	/// </summary>
	/// <remarks>
	/// http://msdn.microsoft.com/en-us/library/aa364698%28v=vs.85%29.aspx
	/// https://www.ietf.org/rfc/rfc1035.txt 2.3.3
	/// </remarks>
	[TypeConverter(typeof(HttpDomain.Converter))]
	public sealed class HttpDomain : LinkPart, IEquatable<HttpDomain>
	{
		public const string WeakWildcardName = "*";
		public const string StrongWildcardName = "+";
		public const string LoopbackName = "127.0.0.1";
		public const string LocalhostName = "localhost";

		public static readonly HttpDomain WeakWildcard = new HttpDomain(WeakWildcardName);
		public static readonly HttpDomain StrongWildcard = new HttpDomain(StrongWildcardName);
		public static readonly HttpDomain Loopback = new HttpDomain(LoopbackName);
		public static readonly HttpDomain Localhost = new HttpDomain(LocalhostName);

		private HttpDomain(LinkText name)
		{
			Name = name;
		}

		public LinkText Name { get; private set; }
		public bool IsWeakWildcard { get { return Name == WeakWildcardName; } }
		public bool IsStrongWildcard { get { return Name == StrongWildcardName; } }
		public bool IsLoopback { get { return Name == LoopbackName; } }
		public bool IsLocalhost { get { return Name.Value.Equals(LocalhostName, StringComparison.OrdinalIgnoreCase); } }
		public bool IsLocal { get { return IsLoopback || IsLocalhost; } }
		public override bool IsTemplate { get { return Name.IsTemplate; } }

		public override Text ToText()
		{
			return Name.ToText();
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpDomain);
		}

		public bool Equals(HttpDomain other)
		{
			return Equality.Check(this, other).Check(x => x.Name);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(HttpDomain x, HttpDomain y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(HttpDomain x, HttpDomain y)
		{
			return !(x == y);
		}

		//
		// Factory
		//

		public static HttpDomain From(LinkText name)
		{
			return new HttpDomain(name);
		}

		public sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value.ToString());
			}
		}
	}
}