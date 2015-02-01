using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Notion"/> class
	/// </summary>
	public class NotionScenarios : Scenarios
	{
		private sealed class TestNotion : Notion
		{
			internal DateTime Now { get { return Clock.Now; } }
		}

		void DefaultTags()
		{
			var tags = (new TestNotion() as ITaggable).Tags;

			Expect(tags.Count).Is(0);
		}

		void SetTag()
		{
			var tags = (new TestNotion() as ITaggable).Tags;

			tags.Set(Notion.Traits.Clock, null);

			Expect(tags.Count).Is(1);
			Expect(tags.Get(Notion.Traits.Clock)).IsNull();
		}

		void DefaultClock()
		{
			var notion = new TestNotion();

			var tags = (notion as ITaggable).Tags;

			var now = new TestNotion().Now;

			Expect(now.Kind).Is(DateTimeKind.Utc);
			Expect(tags.Count).Is(0);
			Expect(tags.Get(Notion.Traits.Clock)).IsAssignableTo(typeof(IClock));
		}
	}
}