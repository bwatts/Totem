using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Expect"/> class
	/// </summary>
	public class ExpectSpecs : Specs
  {
		//
		// Null
		//

		void IsNull()
		{
			Totem.Expect.True<string>(null).IsNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.True("").IsNull());
		}

		void IsNotNull()
		{
			Totem.Expect.True("").IsNotNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.True<string>(null).IsNotNull());
		}

		void IsNullNullable()
		{
			Expect<int?>(null).IsNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.True<int?>(0).IsNull());
		}

		void IsNotNullNullable()
		{
			Totem.Expect.True("").IsNotNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.True<int?>(null).IsNotNull());
		}

		//
		// Equality
		//

		void Is()
		{
			Totem.Expect.True(1).Is(1);

			Assert.Throws<ExpectException>(() => Totem.Expect.True(1).Is(2));
		}

		void IsWithComparer()
		{
			Totem.Expect.True(1).Is(1, new IsComparer { Result = true });

			Assert.Throws<ExpectException>(() => Totem.Expect.True(1).Is(2, new IsComparer { Result = false }));
		}

		void IsNot()
		{
			Totem.Expect.True(1).IsNot(2);

			Assert.Throws<ExpectException>(() => Totem.Expect.True(1).IsNot(1));
		}

		void IsNotWithComparer()
		{
			Totem.Expect.True(1).IsNot(2, new IsComparer { Result = false });

			Assert.Throws<ExpectException>(() => Totem.Expect.True(1).IsNot(1, new IsComparer { Result = true }));
		}

		class IsComparer : IEqualityComparer<int>
		{
			internal bool Result;

			public bool Equals(int x, int y)
			{
				return Result;
			}

			public int GetHashCode(int obj)
			{
				throw new InvalidOperationException();
			}
		}

		//
		// Boolean
		//

		void IsTrue()
		{
			Totem.Expect.True<bool>(true).IsTrue();

			Assert.Throws<ExpectException>(() => Totem.Expect.True<bool>(false).IsTrue());
		}

		void IsFalse()
		{
			Totem.Expect.True<bool>(false).IsFalse();

			Assert.Throws<ExpectException>(() => Totem.Expect.True<bool>(true).IsFalse());
		}

		//
		// Strings
		//

		void IsEmpty()
		{
			Totem.Expect.True("").IsEmpty();

			Assert.Throws<ExpectException>(() => Totem.Expect.True(" ").IsEmpty());
		}

		void IsNotEmpty()
		{
			Totem.Expect.True(" ").IsNotEmpty();

			Assert.Throws<ExpectException>(() => Totem.Expect.True("").IsNotEmpty());
		}
	}
}