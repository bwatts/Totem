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

    public static bool TryFrom(HttpHost host, string resource, out HttpLink link)
    {
      link = HttpResource.TryFrom(resource, out var parsedResource) ? new HttpLink(host, parsedResource) : null;

      return link != null;
    }

    public static bool TryFrom(string host, HttpResource resource, out HttpLink link)
    {
      link = HttpHost.TryFrom(host, out var parsedHost) ? new HttpLink(parsedHost, resource) : null;

      return link != null;
    }

    public static bool TryFrom(string host, string resource, out HttpLink link)
    {
      link = null;

      if(HttpHost.TryFrom(host, out var parsedHost))
      {
        if(HttpResource.TryFrom(resource, out var parsedResource))
        {
          link = new HttpLink(parsedHost, parsedResource);
        }
      }

      return link != null;
    }

    public static bool TryFrom(string value, out HttpLink link)
    {
      link = null;

      var schemeParts = value.Split(new[] { Uri.SchemeDelimiter }, StringSplitOptions.None);

      if(schemeParts.Length == 2)
      {
        var pathSeparatorIndex = schemeParts[1].IndexOf('/');

        if(pathSeparatorIndex == -1 || pathSeparatorIndex == schemeParts[1].Length - 1)
        {
          if(HttpHost.TryFrom(value, out var parsedHost))
          {
            link = new HttpLink(parsedHost, HttpResource.Root);
          }
        }
        else
        {
          var hostText = schemeParts[0] + Uri.SchemeDelimiter + schemeParts[1].Substring(0, pathSeparatorIndex);
          var resourceText = schemeParts[1].Substring(pathSeparatorIndex + 1);

          if(HttpHost.TryFrom(hostText, out var parsedHost))
          {
            if(HttpResource.TryFrom(resourceText, out var parsedResource))
            {
              link = new HttpLink(parsedHost, parsedResource);
            }
          }
        }
      }

      return link != null;
    }

    public static HttpLink From(HttpHost host, HttpResource resource) =>
      new HttpLink(host, resource);

    public static HttpLink From(HttpHost host) =>
      From(host, HttpResource.Root);

    public static HttpLink From(HttpHost host, string resource)
    {
      if(!TryFrom(host, resource, out var link))
      {
        throw new FormatException($"Failed to parse resource: {resource}");
      }

      return link;
    }

    public static HttpLink From(string host, HttpResource resource)
    {
      if(!TryFrom(host, resource, out var link))
      {
        throw new FormatException($"Failed to parse host: {host}");
      }

      return link;
    }

    public static HttpLink From(string host, string resource)
    {
      if(!TryFrom(host, resource, out var link))
      {
        throw new FormatException($"Failed to parse link: {host} {resource}");
      }

      return link;
    }

    public new static HttpLink From(string value)
    {
      if(!TryFrom(value, out var link))
      {
        throw new FormatException($"Failed to parse link: {value}");
      }

      return link;
    }

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}