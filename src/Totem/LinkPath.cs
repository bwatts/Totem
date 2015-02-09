using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// The path to a resource targeted by a link
	/// </summary>
	public sealed class LinkPath : LinkPart, IEquatable<LinkPath>
	{
		public static readonly LinkPath Root = new LinkPath(new List<LinkText>());

		private readonly bool _isTemplate;

		private LinkPath(IReadOnlyList<LinkText> segments)
		{
			Segments = segments;

			_isTemplate = segments.Any(segment => segment.IsTemplate);
		}

		public IReadOnlyList<LinkText> Segments { get; private set; }
		public override bool IsTemplate { get { return _isTemplate; } }

		public sealed override Text ToText()
		{
			return ToText();
		}

		public Text ToText(string separator = "/", bool leading = false, bool trailing = false)
		{
			return Text.None
				.WriteIf(leading, separator)
				.WriteIf(Segments.Any(), Segments.ToTextSeparatedBy(separator))
				.WriteIf(trailing && (!leading || Segments.Any()), separator);
		}

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

		public LinkPath Up(int count = 1, bool strict = true)
		{
			if(Segments.Count < count)
			{
				Expect(strict).IsFalse("Cannot move up from root");

				return Root;
			}

			return new LinkPath(Segments.Take(Segments.Count - count).ToList());
		}

		public LinkPath Then(LinkPath path)
		{
			return path == Root ? this : new LinkPath(Segments.Concat(path.Segments).ToList());
		}

		//
		// Equality
		//

		public override bool Equals(object obj)
		{
			return Equals(obj as LinkPath);
		}

		public bool Equals(LinkPath other)
		{
			return !ReferenceEquals(other, null) && Segments.SequenceEqual(other.Segments);
		}

		public override int GetHashCode()
		{
			return HashCode.CombineItems(Segments);
		}

		public static bool operator ==(LinkPath x, LinkPath y)
		{
			return Equality.CheckOp(x, y);
		}

		public static bool operator !=(LinkPath x, LinkPath y)
		{
			return !(x == y);
		}

		//
		// Factory
		//

		public static LinkPath From(IReadOnlyList<LinkText> segments)
		{
			return new LinkPath(segments);
		}

		public static LinkPath From(IEnumerable<LinkText> segments)
		{
			return From(segments.ToList());
		}

		public static LinkPath From(params LinkText[] segments)
		{
			return From(segments as IEnumerable<LinkText>);
		}

		public static LinkPath From(IEnumerable<string> segments)
		{
			return From(segments.Select(segment => new LinkText(segment)));
		}

		public static LinkPath From(params string[] segments)
		{
			return From(segments as IEnumerable<string>);
		}

		public static LinkPath From(string text, string[] separators)
		{
			var segments = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

			var lastSegment = segments.LastOrDefault();

			return segments.Length == 0 || (segments.Length == 1 && (lastSegment == "" || separators.Contains(lastSegment)))
				? Root
				: From(segments);
		}
	}
}