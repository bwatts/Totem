using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
  /// <summary>
  /// The path to a resource targeted by a link
  /// </summary>
  public sealed class LinkPath : LinkPart, IEquatable<LinkPath>
  {
    readonly bool _isTemplate;

    LinkPath(IReadOnlyList<LinkText> segments)
    {
      Segments = segments;

      _isTemplate = segments.Any(segment => segment.IsTemplate);
    }

    public readonly IReadOnlyList<LinkText> Segments;

    public override bool IsTemplate =>
      _isTemplate;

    public sealed override string ToString() =>
      ToString();

    public string ToString(string separator = "/", bool leading = false, bool trailing = false) =>
      Text
        .If(leading, separator)
        .WriteIf(Segments.Any(), Segments.ToTextSeparatedBy(separator))
        .WriteIf(trailing && (!leading || Segments.Any()), separator);

    public LinkPath RelativeTo(LinkPath other)
    {
      var relativeSegments = new List<LinkText>();

      var foundDifference = false;

      for(var i = 0; i < Segments.Count; i++)
      {
        if(!foundDifference)
        {
          foundDifference = i >= other.Segments.Count || Segments[i] != other.Segments[i];
        }

        if(foundDifference)
        {
          relativeSegments.Add(Segments[i]);
        }
      }

      return new LinkPath(relativeSegments);
    }

    public bool TryUp(out LinkPath path, int count = 1)
    {
      path = Segments.Count <= count ? null : new LinkPath(Segments.Take(Segments.Count - count).ToList());

      return path != null;
    }

    public LinkPath Up(int count = 1)
    {
      if(!TryUp(out var path, count))
      {
        throw new InvalidOperationException("Cannot go up from root");
      }

      return path;
    }

    public LinkPath Then(LinkPath path) =>
      path == Root ? this : new LinkPath(Segments.Concat(path.Segments).ToList());

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as LinkPath);

    public bool Equals(LinkPath other) =>
      !(other is null) && Segments.SequenceEqual(other.Segments);

    public override int GetHashCode() =>
      HashCode.CombineItems(Segments);

    public static bool operator ==(LinkPath x, LinkPath y) => Eq.Op(x, y);
    public static bool operator !=(LinkPath x, LinkPath y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static readonly LinkPath Root = new LinkPath(new List<LinkText>());

    public static LinkPath From(IReadOnlyList<LinkText> segments) =>
      new LinkPath(segments);

    public static LinkPath From(IEnumerable<LinkText> segments) =>
      From(segments.ToList());

    public static LinkPath From(params LinkText[] segments) =>
      From(segments as IEnumerable<LinkText>);

    public static LinkPath From(IEnumerable<string> segments) =>
      From(segments.Select(segment => new LinkText(segment)));

    public static LinkPath From(params string[] segments) =>
      From(segments as IEnumerable<string>);

    public static LinkPath From(string value, string[] separators)
    {
      var segments = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

      var lastSegment = segments.LastOrDefault();

      return segments.Length == 0 || (segments.Length == 1 && (lastSegment == "" || separators.Contains(lastSegment)))
        ? Root
        : From(segments);
    }
  }
}