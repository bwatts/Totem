using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The key and position of a view on the timeline
	/// </summary>
	public sealed class ViewETag : Notion, IEquatable<ViewETag>
	{
		private ViewETag(FlowKey key, TimelinePosition checkpoint)
		{
			Key = key;
			Checkpoint = checkpoint;
		}

		public readonly FlowKey Key;
		public readonly TimelinePosition Checkpoint;

		public override Text ToText()
		{
			return Key.ToText().WriteIf(Checkpoint.IsSome, $"@{Checkpoint.ToInt64OrNull()}");
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ViewETag);
		}

		public bool Equals(ViewETag other)
		{
			return Eq.Values(this, other).Check(x => x.Key).Check(x => x.Checkpoint);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Key, Checkpoint);
		}

		public static bool operator ==(ViewETag x, ViewETag y) => Eq.Op(x, y);
		public static bool operator !=(ViewETag x, ViewETag y) => Eq.OpNot(x, y);

		public static ViewETag From(FlowKey key, TimelinePosition position)
		{
			return new ViewETag(key, position);
		}

		public static ViewETag From(string etag, bool strict = true)
		{
			var parts = etag.Split('@');

			if(parts.Length > 0)
			{
				var key = FlowKey.From(parts[0], strict);

				if(key != null)
				{
					var checkpoint = TimelinePosition.None;

					if(parts.Length == 2)
					{
						long position;

						if(long.TryParse(parts[1], out position))
						{
							checkpoint = new TimelinePosition(position);
						}
					}

					return new ViewETag(key, checkpoint);
				}
			}

			ExpectNot(strict, $"Failed to parse ETag: {etag}");

			return null;
		}
	}
}