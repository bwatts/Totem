using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Eq"/> class
	/// </summary>
	public class EqSpecs : Specs
	{
		void NoValues()
		{
			ExpectThrows<InvalidOperationException>(() => (bool) Eq.Values(1, 1));
		}

		void ValueEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 1 };

			var check = Eq.Values(left, right).Check(x => x.Value);

			Expect((bool) check);
		}

		void ValueNotEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 2 };

			var check = Eq.Values(left, right).Check(x => x.Value);

			ExpectNot((bool) check);
		}

		void ValuesEqual()
		{
			var left = new { ValueA = 1, ValueB = 10 };
			var right = new { ValueA = 1, ValueB = 10 };

			var check = Eq.Values(left, right).Check(x => x.ValueA).Check(x => x.ValueB);

			Expect((bool) check);
		}

		void ValuesNotEqual()
		{
			var left = new { ValueA = 1, ValueB = 10 };
			var right = new { ValueA = 1, ValueB = 11 };

			var check = Eq.Values(left, right).Check(x => x.ValueA).Check(x => x.ValueB);

			ExpectNot((bool) check);
		}

		void OpEqual()
		{
			Expect(Eq.Op(1, 1));
		}

		void OpNotEqual()
		{
			ExpectNot(Eq.Op(1, 2));
		}
	}
}