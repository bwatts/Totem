using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Equality"/> class
	/// </summary>
	public class EqualityScenarios : Scenarios
	{
		//
		// Check
		//

		void CheckNone()
		{
			ExpectThrows<InvalidOperationException>(() => (bool) Equality.Check(1, 1));
		}

		void CheckEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 1 };

			var check = Equality.Check(left, right).Check(x => x.Value);

			Expect((bool) check).IsTrue();
		}

		void CheckNotEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 2 };

			var check = Equality.Check(left, right).Check(x => x.Value);

			Expect((bool) check).IsFalse();
		}

		void CheckOpEqual()
		{
			Expect(Equality.CheckOp(1, 1)).IsTrue();
		}

		void CheckOpNotEqual()
		{
			Expect(Equality.CheckOp(1, 2)).IsFalse();
		}

		//
		// CheckMany
		//

		void CheckManyEqual()
		{
			var left = new { ValueA = 1, ValueB = 10 };
			var right = new { ValueA = 1, ValueB = 10 };

			var check = Equality.Check(left, right).Check(x => x.ValueA).Check(x => x.ValueB);

			Expect((bool) check).IsTrue();
		}

		void CheckManyNotEqual()
		{
			var left = new { ValueA = 1, ValueB = 10 };
			var right = new { ValueA = 1, ValueB = 11 };

			var check = Equality.Check(left, right).Check(x => x.ValueA).Check(x => x.ValueB);

			Expect((bool) check).IsFalse();
		}

		//
		// Compare
		//

		void CompareNone()
		{
			ExpectThrows<InvalidOperationException>(() => (int) Equality.Compare(1, 1));
		}

		void CompareEqual()
		{
			var left = new { Value = 1 };
			var right = new { Value = 1 };

			var compare = Equality.Compare(left, right).Check(x => x.Value);

			Expect((int) compare).Is(0);
		}

		void CompareLessThan()
		{
			var left = new { Value = 1 };
			var right = new { Value = 2 };

			var compare = Equality.Compare(left, right).Check(x => x.Value);

			Expect((int) compare).Is(-1);
		}

		void CompareGreaterThan()
		{
			var left = new { Value = 2 };
			var right = new { Value = 1 };

			var compare = Equality.Compare(left, right).Check(x => x.Value);

			Expect((int) compare).Is(1);
		}
	}
}