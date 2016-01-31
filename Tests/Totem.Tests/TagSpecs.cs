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
		static Tag<int> TagWithoutDefault = Tag.Declare(() => TagWithoutDefault);
		static Tag<int> TagWithDefault = Tag.Declare(() => TagWithDefault, 1);
		static Tag<int> TagWithDefaultResolved = Tag.Declare(() => TagWithDefaultResolved, () => 1);

		void Declare()
		{
			Expect(TagWithoutDefault.Field.DeclaringType).Is(typeof(TagSpecs));
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
		}

		void DeclareWithoutDefault()
		{
			Expect(TagWithoutDefault.Field.Name).Is("TagWithoutDefault");
			Expect(TagWithoutDefault.ResolveDefault()).Is(0);
		}

		void DeclareWithDefault()
		{
			Expect(TagWithDefault.Field.Name).Is("TagWithDefault");
			Expect(TagWithDefault.ResolveDefault()).Is(1);
		}

		void DeclareWithDefaultResolved()
		{
			Expect(TagWithDefaultResolved.Field.Name).Is("TagWithDefaultResolved");
			Expect(TagWithDefaultResolved.ResolveDefault()).Is(1);
		}
	}
}