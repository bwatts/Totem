using System;
using System.ComponentModel;

namespace Totem.IO
{
  /// <summary>
  /// A resource targeted by an HTTP link
  /// </summary>
  [TypeConverter(typeof(Converter))]
  public sealed class HttpResource : Href, IEquatable<HttpResource>
  {
    public const char PathSeparator = '/';
    public const char QuerySeparator = '?';

    HttpResource(LinkPath path, HttpQuery query)
    {
      Path = path;
      Query = query;
    }

    public readonly LinkPath Path;
    public readonly HttpQuery Query;
    public override bool IsTemplate => Path.IsTemplate || Query.IsTemplate;

    public override string ToString() =>
      ToString();

    public string ToString(bool leadingSlash = false, bool trailingSlash = false) =>
      Text
      .Of(Path.ToString(leading: leadingSlash, trailing: trailingSlash))
      .WriteIf(Query.Count > 0, Text.Of(QuerySeparator).Write(Query));

    public HttpResource RelativeTo(HttpResource other)
    {
      Expect.True(Query.IsEmpty, "Queried resources are final");

      return new HttpResource(Path.RelativeTo(other.Path), other.Query);
    }

    public bool TryUp(out HttpResource up, int count = 1)
    {
      up = Path.TryUp(out var pathUp, count) ? new HttpResource(pathUp, Query) : null;

      return up != null;
    }

    public HttpResource Up(int count = 1) =>
      new HttpResource(Path.Up(count), Query);

    public HttpResource Then(HttpResource resource)
    {
      Expect.True(Query.IsEmpty, "Queried resources are final");

      return new HttpResource(Path.Then(resource.Path), resource.Query);
    }

    public HttpResource WithQuery(LinkText key, LinkText value) =>
      new HttpResource(Path, Query.Set(key, value));

    public HttpResource WithoutQuery(LinkText key) =>
      new HttpResource(Path, Query.Clear(key));

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as HttpResource);

    public bool Equals(HttpResource other) =>
      Eq.Values(this, other).Check(x => x.Path).Check(x => x.Query);

    public override int GetHashCode() =>
      HashCode.Combine(Path, Query);

    public static bool operator ==(HttpResource x, HttpResource y) => Eq.Op(x, y);
    public static bool operator !=(HttpResource x, HttpResource y) => Eq.OpNot(x, y);

    //
    // Factory
    //
    
    static readonly string[] _pathSeparators = new[] { PathSeparator.ToString() };

    public static readonly HttpResource Root = new HttpResource(LinkPath.Root, HttpQuery.Empty);

    public static bool TryFrom(LinkPath path, string query, out HttpResource resource)
    {
      resource = HttpQuery.TryFrom(query, out var parsedQuery) ? new HttpResource(path, parsedQuery) : null;

      return resource != null;
    }

    public static bool TryFrom(string path, string query, out HttpResource resource) =>
      TryFrom(LinkPath.From(path, _pathSeparators), query, out resource);

    public static bool TryFrom(string value, out HttpResource resource)
    {
      var parts = value.Split(QuerySeparator);

      var path = LinkPath.From(parts[0], _pathSeparators);

      if(parts.Length == 1)
      {
        resource = new HttpResource(path, HttpQuery.Empty);
      }
      else if(HttpQuery.TryFrom(parts[1], out var parsedQuery))
      {
        resource = new HttpResource(path, parsedQuery);
      }
      else
      {
        resource = null;
      }

      return resource != null;
    }

    public static HttpResource From(LinkPath path, HttpQuery query) =>
      new HttpResource(path, query);

    public static HttpResource From(LinkPath path, string query) =>
      new HttpResource(path, HttpQuery.From(query));

    public static HttpResource From(string path, HttpQuery query) =>
      new HttpResource(LinkPath.From(path, _pathSeparators), query);

    public static HttpResource From(string path, string query) =>
      From(path, HttpQuery.From(query));

    public static HttpResource From(LinkPath path) =>
      From(path, HttpQuery.Empty);

    public new static HttpResource From(string value)
    {
      if(!TryFrom(value, out var resource))
      {
        throw new FormatException($"Failed to parse resource: {value}");
      }

      return resource;
    }

    public static HttpResource From(FolderResource folder) =>
      From(folder.ToString(altSlash: true));

    public static HttpResource From(FileResource file) =>
      From(file.ToString(altSlash: true));

    public new sealed class Converter : TextConverter
    {
      protected override object ConvertFrom(TextValue value) =>
        From(value);
    }
  }
}