using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem.Http
{
	/// <summary>
	/// A link to an HTTP resource
	/// </summary>
	[TypeConverter(typeof(HttpLink.Converter))]
	public sealed class HttpLink : Href, IEquatable<HttpLink>, IComparable<HttpLink>
	{
		private HttpLink(HttpHost host, HttpResource resource)
		{
			Host = host;
			Resource = resource;
		}

		public HttpHost Host { get; private set; }
		public HttpResource Resource { get; private set; }
		public override bool IsTemplate { get { return Host.IsTemplate || Resource.IsTemplate; } }

		public override Text ToText()
		{
			return ToText(false);
		}

		public Text ToText(bool trailingSlash = false)
		{
			return Host.ToText().Write(Resource.ToText(leadingSlash: true, trailingSlash: trailingSlash));
		}

		public HttpLink Then(HttpResource resource)
		{
			return new HttpLink(Host, Resource.Then(resource));
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpLink);
		}

		public bool Equals(HttpLink other)
		{
			return Equality.Check(this, other).Check(x => x.Host).Check(x => x.Resource);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Host, Resource);
		}

		public int CompareTo(HttpLink other)
		{
			return Equality.Compare(this, other).Check(x => x.Host).Check(x => x.Resource);
		}

		//
		// Operators
		//

		public static bool operator ==(HttpLink x, HttpLink y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(HttpLink x, HttpLink y)
		{
			return !(x == y);
		}

		public static bool operator >(HttpLink x, HttpLink y)
		{
			return Equality.CompareOp(x, y) > 0;
		}

		public static bool operator <(HttpLink x, HttpLink y)
		{
			return Equality.CompareOp(x, y) < 0;
		}

		public static bool operator >=(HttpLink x, HttpLink y)
		{
			return Equality.CompareOp(x, y) >= 0;
		}

		public static bool operator <=(HttpLink x, HttpLink y)
		{
			return Equality.CompareOp(x, y) <= 0;
		}

		//
		// Factory
		//

		public static HttpLink From(HttpHost host, HttpResource resource)
		{
			return new HttpLink(host, resource);
		}

		public static HttpLink From(HttpHost host, string resource, bool strict = true)
		{
			var parsedResource = HttpResource.From(resource, strict);

			return parsedResource == null ? null : new HttpLink(host, parsedResource);
		}

		public static HttpLink From(string host, HttpResource resource, bool strict = true)
		{
			var parsedHost = HttpHost.From(host, strict);

			return parsedHost == null ? null : new HttpLink(parsedHost, resource);
		}

		public static HttpLink From(string host, string resource, bool strict = true)
		{
			var parsedHost = HttpHost.From(host, strict);

			return parsedHost == null ? null : From(parsedHost, resource, strict);
		}

		public static HttpLink From(HttpHost host)
		{
			return From(host, HttpResource.Root);
		}

		public new static HttpLink From(string value, bool strict = true)
		{
			var schemeParts = value.Split(new[] { Uri.SchemeDelimiter }, StringSplitOptions.None);

			if(schemeParts.Length == 2)
			{
				var pathSeparatorIndex = schemeParts[1].IndexOf('/');

				if(pathSeparatorIndex == -1 || pathSeparatorIndex == schemeParts[1].Length - 1)
				{
					var host = HttpHost.From(value, strict: false);

					if(host != null)
					{
						return new HttpLink(host, HttpResource.Root);
					}
				}
				else
				{
					var hostText = schemeParts[0] + Uri.SchemeDelimiter + schemeParts[1].Substring(0, pathSeparatorIndex);
					var resourceText = schemeParts[1].Substring(pathSeparatorIndex + 1);

					var host = HttpHost.From(hostText, strict: false);

					if(host != null)
					{
						var resource = HttpResource.From(resourceText, strict: false);

						if(resource != null)
						{
							return new HttpLink(host, resource);
						}
					}
				}
			}

			ExpectNot(strict, "Failed to parse HTTP link: " + value);

			return null;
		}

		public new sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}