using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Field"/> class
	/// </summary>
	public class FieldSpecs : Specs
  {
		static Field<int> FieldWithoutDefault = Field.Declare(() => FieldWithoutDefault);
		static Field<int> FieldWithDefault = Field.Declare(() => FieldWithDefault, 1);
		static Field<int> FieldWithDefaultResolved = Field.Declare(() => FieldWithDefaultResolved, () => 1);

		void Declare()
		{
			Expect(FieldWithoutDefault.Info.DeclaringType).Is(typeof(FieldSpecs));
			Expect(FieldWithoutDefault.Info.Name).Is("FieldWithoutDefault");
		}

		void DeclareWithoutDefault()
		{
			Expect(FieldWithoutDefault.Info.Name).Is("FieldWithoutDefault");
			Expect(FieldWithoutDefault.ResolveDefault()).Is(0);
		}

		void DeclareWithDefault()
		{
			Expect(FieldWithDefault.Info.Name).Is("FieldWithDefault");
			Expect(FieldWithDefault.ResolveDefault()).Is(1);
		}

		void DeclareWithDefaultResolved()
		{
			Expect(FieldWithDefaultResolved.Info.Name).Is("FieldWithDefaultResolved");
			Expect(FieldWithDefaultResolved.ResolveDefault()).Is(1);
		}
	}
}