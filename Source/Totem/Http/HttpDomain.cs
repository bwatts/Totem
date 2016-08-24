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
	[TypeConverter(typeof(Converter))]
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
		public bool IsWeakWildcard => Name == WeakWildcardName;
		public bool IsStrongWildcard => Name == StrongWildcardName;
		public bool IsLoopback => Name == LoopbackName;
		public bool IsLocalhost => Name.Value.Equals(LocalhostName, StringComparison.OrdinalIgnoreCase);
		public bool IsLocal => IsLoopback || IsLocalhost;
		public override bool IsTemplate => Name.IsTemplate;

		public override Text ToText() => Name.ToText();

		public HttpDomain ResolveWildcard(HttpDomain domain)
		{
			return IsWeakWildcard || IsStrongWildcard ? domain : this;
		}

		public HttpDomain ResolveWildcard(string domain)
		{
			return IsWeakWildcard || IsStrongWildcard ? From(domain) : this;
		}

		public HttpDomain ResolveWildcard()
		{
			return IsWeakWildcard || IsStrongWildcard ? Localhost : this;
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
			return Eq.Values(this, other).Check(x => x.Name);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(HttpDomain x, HttpDomain y) => Eq.Op(x, y);
		public static bool operator !=(HttpDomain x, HttpDomain y) => Eq.OpNot(x, y);

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