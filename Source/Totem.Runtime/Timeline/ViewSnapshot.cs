using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A snapshot of the most recent version of a view
	/// </summary>
	public class ViewSnapshot<TContent> : Notion
	{
		private readonly TContent _content;

		private ViewSnapshot(
			FlowKey key,
			TimelinePosition checkpoint,
			bool notFound = false,
			bool notModified = false,
			TContent content = default(TContent))
		{
			Key = key;
			Checkpoint = checkpoint;
			NotFound = notFound;
			NotModified = notModified;
			_content = content;
		}

		public readonly FlowKey Key;
		public readonly TimelinePosition Checkpoint;
		public readonly bool NotFound;
		public readonly bool NotModified;

		public override Text ToText() => Key.ToText();

		public TContent ReadContent()
		{
			ExpectNot(NotFound, "Cannot read content of view that was not found");
			ExpectNot(NotModified, "Cannot read content of view that was not modified");

			return _content;
		}

		public static ViewSnapshot<TContent> OfNotFound(FlowKey key)
		{
			return new ViewSnapshot<TContent>(key, TimelinePosition.None, notFound: true);
		}

		public static ViewSnapshot<TContent> OfNotModified(FlowKey key, TimelinePosition checkpoint)
		{
			return new ViewSnapshot<TContent>(key, checkpoint, notModified: true);
		}

		public static ViewSnapshot<TContent> OfContent(FlowKey key, TimelinePosition checkpoint, TContent content)
		{
			Expect(checkpoint.IsSome, "A view without a checkpoint cannot have content");

			return new ViewSnapshot<TContent>(key, checkpoint, content: content);
		}
	}
}