using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.TagValue"/> class
	/// </summary>
	public class TagValueSpecs : Specs
  {
		static Tag<int> Tag = Totem.Tag.Declare(() => Tag, 1);

		void WithoutContent()
		{
			var value = new TagValue(Tag);

			Expect(value.Tag).Is(Tag);
			Expect(value.Content).Is(Tag.ResolveDefault());
			Expect(value.IsUnset);
		}

		void WithContent()
		{
			var value = new TagValue(Tag, 2);

			Expect(value.Tag).Is(Tag);
			Expect(value.Content).Is(2);
			ExpectNot(value.IsUnset);
		}
	}
}