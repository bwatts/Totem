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
		class TestNotion : Notion
		{
			internal DateTime Now { get { return Clock.Now; } }
		}

		void Create()
		{
			var tags = (new TestNotion() as ITaggable).Tags;

			Expect(tags.Count).Is(0);
		}

		void GetTagDefault()
		{
			var notion = new TestNotion();
			var tags = (notion as ITaggable).Tags;

			Expect(notion.Now.Kind).Is(DateTimeKind.Utc);

			Expect(tags.Count).Is(1);
			Expect(tags.Get(Notion.Traits.Clock)).IsAssignableTo(typeof(IClock));
		}

		void SetTag()
		{
			var tags = (new TestNotion() as ITaggable).Tags;

			tags.Set(Notion.Traits.Clock, null);

			Expect(tags.Count).Is(1);
			Expect(tags.Get(Notion.Traits.Clock)).IsNull();
		}
	}
}