using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A link to an HTTP resource
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class HttpLink : Href, IEquatable<HttpLink>, IComparable<HttpLink>
  {
    HttpLink(HttpHost host, HttpResource resource)
    {
      Host = host;
      Resource = resource;
    }

    public readonly HttpHost Host;
    public readonly HttpResource Resource;

    public override bool IsTemplate =>
      Host.IsTemplate || Resource.IsTemplate;

    public override string ToString() =>
      ToString();

    public string ToString(bool trailingSlash = false) =>
      Text.Of(Host).Write(Resource.ToString(leadingSlash: true, trailingSlash: trailingSlash));

    public HttpLink Then(HttpResource resource) =>
      new HttpLink(Host, Resource.Then(resource));

    public HttpLink ResolveHostWildcard(HttpDomain domain)
    {
      var resolved = Host.ResolveDomainWildcard(domain);

      return resolved == Host ? this : From(resolved, Resource);
    }

    public HttpLink ResolveHostWildcard(string domain)
    {
      var resolved = Host.ResolveDomainWildcard(domain);

      return resolved == Host ? this : From(resolved, Resource);
    }

    public HttpLink ResolveHostWildcard()
    {
      var resolved = Host.ResolveDomainWildcard();

      return resolved == Host ? this : From(resolved, Resource);
    }

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpLink);

    public bool Equals(HttpLink other) =>
      Eq.Values(this, other).Check(x => x.Host).Check(x => x.Resource);

    public override int GetHashCode() =>
      HashCode.Combine(Host, Resource);

    public int CompareTo(HttpLink other) =>
      Cmp.Values(this, other).Check(x => x.Host).Check(x => x.Resource);

    public static bool operator ==(HttpLink x, HttpLink y) => Eq.Op(x, y);
    public static bool operator !=(HttpLink x, HttpLink y) => Eq.OpNot(x, y);
    public static bool operator >(HttpLink x, HttpLink y) => Cmp.Op(x, y) > 0;
    public static bool operator <(HttpLink x, HttpLink y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(HttpLink x, HttpLink y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(HttpLink x, HttpLink y) => Cmp.Op(x, y) <= 0;

    //
    // Factory
    //

    public static HttpLink From(HttpHost host, HttpResource resource) =>
      new HttpLink(host, resource);

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

    public static HttpLink From(HttpHost host) =>
      From(host, HttpResource.Root);

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

      Expect.False(strict, "Failed to parse HTTP link: " + value);

      return null;
    }

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}