using System;
using System.ComponentModel;

namespace Totem.IO
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

    HttpHost(bool secure, HttpDomain domain, int port, bool portIsDefault)
    {
      Secure = secure;
      Domain = domain;
      Port = port;
      PortIsDefault = portIsDefault;
    }

    public readonly bool Secure;
    public readonly HttpDomain Domain;
    public readonly int Port;
    public readonly bool PortIsDefault;

    public string Scheme =>
      Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

    public override bool IsTemplate =>
      Domain.IsTemplate;

    public override string ToString() =>
      ToString();

    public string ToString(bool defaultPort = false) =>
      Text.None
        .WriteIf(Secure, Uri.UriSchemeHttps, Uri.UriSchemeHttp)
        .Write(Uri.SchemeDelimiter)
        .Write(Domain)
        .WriteIf(defaultPort || !PortIsDefault, PortSeparator + Port.ToString());

    public HttpLink ToLink() =>
      HttpLink.From(this);

    public HttpHost ResolveDomainWildcard(HttpDomain domain)
    {
      var resolved = Domain.ResolveWildcard(domain);

      return resolved == Domain ? this : From(Secure, resolved, Port);
    }

    public HttpHost ResolveDomainWildcard(string domain)
    {
      var resolved = Domain.ResolveWildcard(domain);

      return resolved == Domain ? this : From(Secure, resolved, Port);
    }

    public HttpHost ResolveDomainWildcard()
    {
      var resolved = Domain.ResolveWildcard();

      return resolved == Domain ? this : From(Secure, resolved, Port);
    }

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpHost);

    public bool Equals(HttpHost other) =>
      Eq.Values(this, other).Check(x => x.Secure).Check(x => x.Domain).Check(x => x.Port);

    public override int GetHashCode() =>
      HashCode.Combine(Secure, Domain, Port);

    public static bool operator ==(HttpHost x, HttpHost y) => Eq.Op(x, y);
    public static bool operator !=(HttpHost x, HttpHost y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static bool TryFrom(string value, out HttpHost host)
    {
      host = null;

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
          if(domain.EndsWith("/"))
          {
            domain = domain.Substring(0, domain.Length - 1);
          }

          var domainParts = domain.Split(PortSeparator);

          if(domainParts.Length == 1)
          {
            host = From(isHttps, domainParts[0]);
          }
          else
          {
            if(int.TryParse(domainParts[1], out var port))
            {
              host = From(isHttps, domainParts[0], port);
            }
          }
        }
      }

      return host != null;
    }

    public static HttpHost From(string value)
    {
      if(!TryFrom(value, out var host))
      {
        throw new FormatException($"Failed to parse HTTP host: {value}");
      }

      return host;
    }

    public static HttpHost From(bool secure, HttpDomain domain, int port) =>
      new HttpHost(secure, domain, port, secure ? port == DefaultSecurePort : port == DefaultPort);

    public static HttpHost From(bool secure, HttpDomain domain) =>
      new HttpHost(secure, domain, secure ? DefaultSecurePort : DefaultPort, portIsDefault: true);

    public static HttpHost From(bool secure, string domain, int port) =>
      From(secure, HttpDomain.From(domain), port);

    public static HttpHost From(bool secure, string domain) =>
      From(secure, HttpDomain.From(domain));

    public static HttpHost FromHttp(HttpDomain domain) =>
      From(false, domain);

    public static HttpHost FromHttps(HttpDomain domain) =>
      From(true, domain);

    public static HttpHost FromHttp(string domain) =>
      From(false, domain);

    public static HttpHost FromHttps(string domain) =>
      From(true, domain);

    public sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}