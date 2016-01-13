using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Tag"/> class
	/// </summary>
	public class TagSpecs : Specs
  {
		static Tag TagWithoutValue = Tag.Declare(() => TagWithoutValue);
		static Tag<int> TagWithoutDefault = Tag.Declare(() => TagWithoutDefault);
		static Tag<int> TagWithDefault = Tag.Declare(() => TagWithDefault, 1);
		static Tag<int> TagWithDefaultResolved = Tag.Declare(() => TagWithDefaultResolved, () => 1);

		void Declare()
		{
			Expect(TagWithoutValue.HasValue).IsFalse("Tag should not have a value");
			Expect(TagWithoutValue.Field.DeclaringType).Is(typeof(TagSpecs));
			Expect(TagWithoutValue.Field.Name).Is("TagWithoutValue");
		}

		void DeclareValue()
		{
			Expect(TagWithoutDefault.HasValue).IsTrue("Tag should have a value");
			Expect(TagWithoutDefault.Field.DeclaringType).Is(typeof(TagSpecs));
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
		}

		void DeclareWithoutValue()
		{
			Expect(TagWithoutValue.Field.Name).Is("TagWithoutValue");
		}

		void DeclareWithoutDefault()
		{
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
			Expect(TagWithoutDefault.ResolveDefaultValue()).Is(0);
		}

		void DeclareWithDefault()
		{
			Expect(TagWithDefault.Field.Name).Is("TagWithDefault");
			Expect(TagWithDefault.ResolveDefaultValue()).Is(1);
		}

		void DeclareWithDefaultResolved()
		{
			Expect(TagWithDefaultResolved.Field.Name).Is("TagWithDefaultResolved");
			Expect(TagWithDefaultResolved.ResolveDefaultValue()).Is(1);
		}
	}
}