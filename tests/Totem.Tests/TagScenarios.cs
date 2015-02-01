using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Tag"/> class
	/// </summary>
	public class TagScenarios : Scenarios
	{
		public static readonly Tag TagWithoutValue = Tag.Declare(() => TagWithoutValue);
		public static readonly Tag<int> TagWithoutDefault = Tag.Declare(() => TagWithoutDefault);
		public static readonly Tag<int> TagWithDefault = Tag.Declare(() => TagWithDefault, 1);
		public static readonly Tag<int> TagWithDefaultResolved = Tag.Declare(() => TagWithDefaultResolved, () => 1);

		void Declare()
		{
			Expect(TagWithoutValue.HasValue).IsFalse("Tag should not have a value");
			Expect(TagWithoutValue.Field.DeclaringType).Is(typeof(TagScenarios));
			Expect(TagWithoutValue.Field.Name).Is("TagWithoutValue");
		}

		void DeclareValue()
		{
			Expect(TagWithoutDefault.HasValue).IsTrue("Tag should have a value");
			Expect(TagWithoutDefault.Field.DeclaringType).Is(typeof(TagScenarios));
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
		}

		void WithoutValue()
		{
			Expect(TagWithoutValue.Field.Name).Is("TagWithoutValue");
		}

		void WithoutDefault()
		{
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
			Expect(TagWithoutDefault.ResolveDefaultValue()).Is(0);
		}

		void WithDefault()
		{
			Expect(TagWithDefault.Field.Name).Is("TagWithDefault");
			Expect(TagWithDefault.ResolveDefaultValue()).Is(1);
		}

		void WithDefaultResolved()
		{
			Expect(TagWithDefaultResolved.Field.Name).Is("TagWithDefaultResolved");
			Expect(TagWithDefaultResolved.ResolveDefaultValue()).Is(1);
		}
	}
}