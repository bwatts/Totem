using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Tags"/> class
	/// </summary>
	public class TagsSpecs : Specs
  {
		static Tag<int> Tag = Totem.Tag.Declare(() => Tag);
		static Tag<string> OtherTag = Totem.Tag.Declare(() => OtherTag);

		void Create()
		{
			var tags = new Tags();

			Expect(tags.Count).Is(0);
			ExpectNot(tags.Any());
			ExpectNot(tags.Keys.Any());
			ExpectNot(tags.Values.Any());
			ExpectThrows<KeyNotFoundException>(() => tags[Tag]);
		}

		void Set()
		{
			var tags = new Tags();

			tags.Set(Tag, 1);

			Expect(tags.Count).Is(1);
			Expect(tags.Any());
			Expect(tags.Keys.Count()).Is(1);
			Expect(tags.Values.Count()).Is(1);
			ExpectNot(tags.IsUnset(Tag));
			Expect(tags[Tag].Content).Is(1);
		}

		void Clear()
		{
			var tags = new Tags();

			tags.Set(Tag, 2);
			tags.Set(OtherTag, "");

			tags.Clear(Tag);

			Expect(tags.Count).Is(1);
			Expect(tags[OtherTag].Content).Is("");
		}

		void GetUnset()
		{
			var tags = new Tags();

			Expect(tags.Get((Tag) Tag)).Is(0);
		}

		void GetUnsetStrict()
		{
			var tags = new Tags();

			ExpectThrows<InvalidOperationException>(() => tags.Get((Tag) Tag, throwIfUnset: true));
		}

		void Get()
		{
			var tags = new Tags();

			tags.Set(Tag, 1);

			Expect(tags.Get(Tag)).Is(1);
		}

		void GetStrict()
		{
			var tags = new Tags();

			tags.Set(Tag, 1);

			Expect(tags.Get(Tag, throwIfUnset: true)).Is(1);
		}
	}
}