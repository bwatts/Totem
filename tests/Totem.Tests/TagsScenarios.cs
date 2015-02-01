using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Tags"/> class
	/// </summary>
	public class TagsScenarios : Scenarios
	{
		static Tag TagWithoutValue = Tag.Declare(() => TagWithoutValue);
		static Tag<int> TagWithValue = Tag.Declare(() => TagWithValue);

		void Create()
		{
			var tags = new Tags();

			Expect(tags.Count).Is(0);
			ExpectTrue(!tags.Any());
			ExpectTrue(!tags.Keys.Any());
			ExpectTrue(!tags.Values.Any());
			ExpectThrows<KeyNotFoundException>(() => tags[TagWithoutValue]);
		}

		void SetWithoutValue()
		{
			var tags = new Tags();

			tags.Set(TagWithoutValue);

			Expect(tags.Count).Is(1);
			ExpectTrue(tags.Any());
			Expect(tags.Keys.Count()).Is(1);
			Expect(tags.Values.Count()).Is(1);
			ExpectTrue(tags.IsSet(TagWithoutValue));
			Expect(tags[TagWithoutValue].Content).Is(Tag.UnsetValue);
		}

		void SetWithValue()
		{
			var tags = new Tags();

			tags.Set(TagWithValue, 1);

			Expect(tags.Count).Is(1);
			ExpectTrue(tags.Any());
			Expect(tags.Keys.Count()).Is(1);
			Expect(tags.Values.Count()).Is(1);
			ExpectTrue(tags.IsSet(TagWithValue));
			Expect(tags[TagWithValue].Content).Is(1);
		}

		void Clear()
		{
			var tags = new Tags();

			tags.Set(TagWithoutValue);
			tags.Set(TagWithValue, 2);

			tags.Clear(TagWithoutValue);

			Expect(tags.Count).Is(1);
			Expect(tags[TagWithValue].Content).Is(2);
		}

		void GetUnset()
		{
			var tags = new Tags();

			Expect(tags.Get(TagWithoutValue)).Is(Tag.UnsetValue);
		}

		void GetUnsetStrict()
		{
			var tags = new Tags();

			ExpectThrows<ExpectException>(() => tags.Get(TagWithoutValue, throwIfUnset: true));
		}

		void GetWithoutValue()
		{
			var tags = new Tags();

			tags.Set(TagWithoutValue);

			Expect(tags.Get(TagWithoutValue)).Is(Tag.UnsetValue);
		}

		void GetWithValue()
		{
			var tags = new Tags();

			tags.Set(TagWithValue, 1);

			Expect(tags.Get(TagWithValue)).Is(1);
		}

		void GetWithoutValueStrict()
		{
			var tags = new Tags();

			tags.Set(TagWithoutValue);

			ExpectThrows<ExpectException>(() => tags.Get(TagWithoutValue, throwIfUnset: true));
		}

		void GetWithValueStrict()
		{
			var tags = new Tags();

			tags.Set(TagWithValue, 1);

			Expect(tags.Get(TagWithValue, throwIfUnset: true)).Is(1);
		}
	}
}