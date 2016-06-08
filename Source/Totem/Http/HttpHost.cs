using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.IO;
using Totem.Reflection;

namespace Totem.Http
{
	/// <summary>
	/// The global identifier of an HTTP host
	/// </summary>
	[TypeConverter(typeof(Converter))]
	public sealed class HttpHost : LinkPart, IEquatable<HttpHost>
	{
		public const char PortSeparator = ':';
		public const int DefaultPort = 80;
		public const int DefaultSecurePort = 443;

		private HttpHost(bool secure, HttpDomain domain, int port, bool portIsDefault)
		{
			Secure = secure;
			Domain = domain;
			Port = port;
			PortIsDefault = portIsDefault;
		}

		public bool Secure { get; private set; }
		public HttpDomain Domain { get; private set; }
		public int Port { get; private set; }
		public bool PortIsDefault { get; private set; }
		public string Scheme => Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
		public override bool IsTemplate => Domain.IsTemplate;

		public override Text ToText() => ToText();

		public Text ToText(bool defaultPort = false)
		{
			return Text.None
				.WriteIf(Secure, Uri.UriSchemeHttps, Uri.UriSchemeHttp)
				.Write(Uri.SchemeDelimiter)
				.Write(Domain)
				.WriteIf(defaultPort || !PortIsDefault, PortSeparator + Port.ToString());
		}

		public HttpLink ToLink() => HttpLink.From(this);

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as HttpHost);
		}

		public bool Equals(HttpHost other)
		{
			return Eq.Values(this, other).Check(x => x.Secure).Check(x => x.Domain).Check(x => x.Port);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Secure, Domain, Port);
		}

		public static bool operator ==(HttpHost x, HttpHost y) => Eq.Op(x, y);
		public static bool operator !=(HttpHost x, HttpHost y) => Eq.OpNot(x, y);

		//
		// Factory
		//

		public static HttpHost From(bool secure, HttpDomain domain, int port)
		{
			return new HttpHost(secure, domain, port, secure ? port == DefaultSecurePort : port == DefaultPort);
		}

		public static HttpHost From(bool secure, HttpDomain domain)
		{
			return new HttpHost(secure, domain, secure ? DefaultSecurePort : DefaultPort, portIsDefault: true);
		}

		public static HttpHost From(bool secure, string domain, int port)
		{
			return From(secure, HttpDomain.From(domain), port);
		}

		public static HttpHost From(bool secure, string domain)
		{
			return From(secure, HttpDomain.From(domain));
		}

		public static HttpHost FromHttp(HttpDomain domain)
		{
			return From(false, domain);
		}

		public static HttpHost FromHttps(HttpDomain domain)
		{
			return From(true, domain);
		}

		public static HttpHost FromHttp(string domain)
		{
			return From(false, domain);
		}

		public static HttpHost FromHttps(string domain)
		{
			return From(true, domain);
		}

		public static HttpHost From(string value, bool strict = true)
		{
			var parts = value.Split(new[] { Uri.SchemeDelimiter }, StringSplitOptions.None);

			if(parts.Length > 0)
			{
				var isHttp = parts[0] == Uri.UriSchemeHttp;
				var isHttps = parts[0] == Uri.UriSchemeHttps;

				string domain;

				if(parts.Length == 1 && !isHttp && !isHttps)
				{
					domain = parts[0];
				}
				else if(parts.Length == 2 && (isHttp || isHttps))
				{
					domain = parts[1];
				}
				else
				{
					domain = null;
				}

				if(domain != null)
				{
					var domainParts = domain.Split(PortSeparator);

					if(domainParts.Length == 1)
					{
						return From(isHttps, domainParts[0]);
					}
					else
					{
						var portText = domainParts[1];

						if(portText.EndsWith("/"))
						{
							portText = portText.Substring(0, portText.Length - 1);
						}

						int port;

						if(int.TryParse(portText, out port))
						{
							return From(isHttps, domainParts[0], port);
						}
					}
				}
			}

			ExpectNot(strict, "Failed to parse host: " + value);

			return null;
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