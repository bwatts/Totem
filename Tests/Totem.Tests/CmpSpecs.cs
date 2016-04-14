using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Cmp"/> class
	/// </summary>
	public class CmpSpecs : Specs
	{
		void NoValues()
		{
			ExpectThrows<InvalidOperationException>(() => (int) Cmp.Values(1, 1));
		}

		void ValueEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 1 };

			var compare = Cmp.Values(left, right).Check(x => x.Value);

			Expect((int) compare).Is(0);
		}

		void ValueLessThan()
		{
			var left = new { Value = 1 };
			var right = new { Value = 2 };

			var compare = Cmp.Values(left, right).Check(x => x.Value);

			Expect((int) compare).Is(-1);
		}

		void ValueGreaterThan()
		{
			var left = new { Value = 2 };
			var right = new { Value = 1 };

			var compare = Cmp.Values(left, right).Check(x => x.Value);

			Expect((int) compare).Is(1);
		}

		void ValuesEqual()
		{
			var left = new { Value1 = 1, Value2 = 2 };
			var right = new { Value1 = 1, Value2 = 2 };

			var compare = Cmp.Values(left, right).Check(x => x.Value1).Check(x => x.Value2);

			Expect((int) compare).Is(0);
		}

		void ValuesLessThan()
		{
			var left = new { Value1 = 1, Value2 = 0 };
			var right = new { Value1 = 1, Value2 = 2 };

			var compare = Cmp.Values(left, right).Check(x => x.Value1).Check(x => x.Value2);

			Expect((int) compare).Is(-1);
		}

		void ValuesGreaterThan()
		{
			var left = new { Value1 = 1, Value2 = 2 };
			var right = new { Value1 = 1, Value2 = 0 };

			var compare = Cmp.Values(left, right).Check(x => x.Value1).Check(x => x.Value2);

			Expect((int) compare).Is(1);
		}

		void OpEqual()
		{
			Expect(Cmp.Op(1, 1)).Is(0);
		}

		void OpLessThan()
		{
			Expect(Cmp.Op(1, 2)).Is(-1);
		}

		void OpGreaterThan()
		{
			Expect(Cmp.Op(2, 1)).Is(1);
		}
	}
}