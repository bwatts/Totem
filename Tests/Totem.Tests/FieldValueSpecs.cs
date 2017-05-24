using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.FieldValue"/> class
	/// </summary>
	public class FieldValueSpecs : Specs
  {
		static Field<int> Field = Totem.Field.Declare(() => Field, 1);

		void WithoutContent()
		{
			var value = new FieldValue(Field);

			Expect(value.Field).Is(Field);
			Expect(value.Content).Is(Field.ResolveDefault());
			Expect(value.IsUnset);
			ExpectNot(value.IsSet);
		}

		void WithContent()
		{
			var value = new FieldValue(Field, 2);

			Expect(value.Field).Is(Field);
			Expect(value.Content).Is(2);
			ExpectNot(value.IsUnset);
			Expect(value.IsSet);
		}
	}
}