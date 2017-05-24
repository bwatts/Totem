using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Notion"/> class
	/// </summary>
	public class NotionSpecs : Specs
  {
		class TestNotion : Notion
		{
			internal DateTime Now => Clock.Now;
		}

		void Create()
		{
			var fields = (new TestNotion() as IBindable).Fields;

			Expect(fields.Count).Is(0);
		}

		void GetTagDefault()
		{
			var notion = new TestNotion();
			var fields = (notion as IBindable).Fields;

			Expect(notion.Now.Kind).Is(DateTimeKind.Utc);

			Expect(fields.Count).Is(1);
			Expect(fields.Get(Notion.Traits.Clock)).IsAssignableTo(typeof(IClock));
		}

		void SetTag()
		{
			var fields = (new TestNotion() as IBindable).Fields;

      fields.Set(Notion.Traits.Clock, null);

			Expect(fields.Count).Is(1);
			Expect(fields.Get(Notion.Traits.Clock)).IsNull();
		}
	}
}