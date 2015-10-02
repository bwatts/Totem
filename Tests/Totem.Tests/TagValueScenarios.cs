using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.TagValue"/> class
	/// </summary>
	public class TagValueScenarios : Scenarios
	{
		static Tag TagWithoutValue = Tag.Declare(() => TagWithoutValue);
		static Tag<int> TagWithValue = Tag.Declare(() => TagWithValue, 1);

		void WithoutValue()
		{
			var value = new TagValue(TagWithoutValue);

			Expect(value.Tag).Is(TagWithoutValue);
			Expect(value.Content).Is(Tag.UnsetValue);
			Expect(value.IsUnset).IsTrue();
		}

		void WithValue()
		{
			var value = new TagValue(TagWithValue, 2);

			Expect(value.Tag).Is(TagWithValue);
			Expect(value.Content).Is(2);
			Expect(value.IsUnset).IsFalse();
		}

		void WithDefaultValue()
		{
			var value = new TagValue(TagWithValue);

			Expect(value.Tag).Is(TagWithValue);
			Expect(value.Content).Is(1);
			Expect(value.IsUnset).IsFalse();
		}
	}
}