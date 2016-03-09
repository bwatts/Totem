using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem.Http
{
	/// <summary>
	/// A resource targeted by an HTTP link
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class HttpResource : Href, IEquatable<HttpResource>
	{
		public const char PathSeparator = '/';
		public const char QuerySeparator = '?';

		public static readonly HttpResource Root = new HttpResource(LinkPath.Root, HttpQuery.Empty);

		private HttpResource(LinkPath path, HttpQuery query)
		{
			Path = path;
			Query = query;
		}

		public LinkPath Path { get; private set; }
		public HttpQuery Query { get; private set; }
		public override bool IsTemplate => Path.IsTemplate || Query.IsTemplate;

		public override Text ToText() => ToText();

		public Text ToText(bool leadingSlash = false, bool trailingSlash = false)
		{
			return Path
				.ToText(leading: leadingSlash, trailing: trailingSlash)
				.WriteIf(Query.Count > 0, Text.Of(QuerySeparator).Write(Query));
		}

		public HttpResource RelativeTo(HttpResource other)
		{
			Expect(Query.IsEmpty, "Queried resources are final");

			return new HttpResource(Path.RelativeTo(other.Path), other.Query);
		}

		public HttpResource Up(int count = 1, bool strict = true)
		{
			return new HttpResource(Path.Up(count, strict), Query);
		}

		public HttpResource Then(HttpResource resource)
		{
			Expect(Query.IsEmpty, "Queried resources are final");

			return new HttpResource(Path.Then(resource.Path), resource.Query);
		}

		public HttpResource WithQuery(LinkText key, LinkText value)
		{
			return new HttpResource(Path, Query.Set(key, value));
		}

		public HttpResource WithoutQuery(LinkText key)
		{
			return new HttpResource(Path, Query.Clear(key));
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpResource);
		}

		public bool Equals(HttpResource other)
		{
			return Eq.Values(this, other).Check(x => x.Path).Check(x => x.Query);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Path, Query);
		}

		public static bool operator ==(HttpResource x, HttpResource y) => Eq.Op(x, y);
		public static bool operator !=(HttpResource x, HttpResource y) => Eq.OpNot(x, y);

		//
		// Factory
		//

		public static HttpResource From(LinkPath path, HttpQuery query)
		{
			return new HttpResource(path, query);
		}

		public static HttpResource From(LinkPath path, string query, bool strict = true)
		{
			var parsedQuery = HttpQuery.From(query, strict);

			return parsedQuery == null ? null : new HttpResource(path, parsedQuery);
		}

		public static HttpResource From(string path, HttpQuery query)
		{
			return new HttpResource(LinkPath.From(path, _pathSeparators), query);
		}

		public static HttpResource From(string path, string query, bool strict = true)
		{
			return From(LinkPath.From(path, _pathSeparators), query);
		}

		public static HttpResource From(LinkPath path)
		{
			return From(path, HttpQuery.Empty);
		}

		public new static HttpResource From(string value, bool strict = true)
		{
			var parts = value.Split(QuerySeparator);

			if(parts.Length == 1)
			{
				return new HttpResource(LinkPath.From(parts[0], _pathSeparators), HttpQuery.Empty);
			}
			else
			{
				var path = LinkPath.From(parts[0], _pathSeparators);
				var query = HttpQuery.From(parts[1], strict);

				if(query != null)
				{
					return new HttpResource(path, query);
				}
			}

			ExpectNot(strict, "Failed to parse resource: " + value);

			return null;
		}

		private static readonly string[] _pathSeparators = new[] { PathSeparator.ToString() };

		public new sealed class Converter : TextConverter
		{
			protected override object ConvertFrom(TextValue value)
			{
				return From(value);
			}
		}
	}
}