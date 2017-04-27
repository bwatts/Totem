using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Fields"/> class
	/// </summary>
	public class FieldsSpecs : Specs
  {
    class Binding : IBindable
    {
      public Fields Fields { get; } = new Fields();
    }

		static Field<int> Field = Totem.Field.Declare(() => Field);
		static Field<string> OtherField = Totem.Field.Declare(() => OtherField);

		void Create()
		{
      var fields = new Binding().Fields;

			Expect(fields.Count).Is(0);
			ExpectNot(fields.Any());
			ExpectNot(fields.Keys.Any());
			ExpectNot(fields.Values.Any());
			ExpectThrows<KeyNotFoundException>(() => fields[Field]);
		}

		void Set()
		{
			var fields = new Binding().Fields;

			fields.Set(Field, 1);

			Expect(fields.Count).Is(1);
			Expect(fields.Any());
			Expect(fields.Keys.Count()).Is(1);
			Expect(fields.Values.Count()).Is(1);
			ExpectNot(fields.IsUnset(Field));
			Expect(fields.IsSet(Field));
			Expect(fields[Field].Content).Is(1);
		}

		void Clear()
		{
			var fields = new Binding().Fields;

			fields.Set(Field, 2);
			fields.Set(OtherField, "");

			fields.Clear(Field);

			Expect(fields.Count).Is(1);
			Expect(fields[OtherField].Content).Is("");
		}

		void GetUnset()
		{
			var fields = new Binding().Fields;

			Expect(fields.Get((Field) Field)).Is(0);
		}

		void GetUnsetStrict()
		{
			var fields = new Binding().Fields;

			ExpectThrows<Exception>(() => fields.Get((Field) Field, throwIfUnset: true));
		}

		void Get()
		{
			var fields = new Binding().Fields;

			fields.Set(Field, 1);

			Expect(fields.Get(Field)).Is(1);
		}

		void GetStrict()
		{
			var fields = new Binding().Fields;

      fields.Set(Field, 1);

			Expect(fields.Get(Field, throwIfUnset: true)).Is(1);
		}
	}
}