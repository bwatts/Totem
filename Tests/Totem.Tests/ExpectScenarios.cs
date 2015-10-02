using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Expect"/> class
	/// </summary>
	public class ExpectScenarios : Scenarios
	{
		//
		// Null
		//

		void IsNull()
		{
			Totem.Expect.That<string>(null).IsNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.That("").IsNull());
		}

		void IsNotNull()
		{
			Totem.Expect.That("").IsNotNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.That<string>(null).IsNotNull());
		}

		void IsNullNullable()
		{
			Expect<int?>(null).IsNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.That<int?>(0).IsNull());
		}

		void IsNotNullNullable()
		{
			Totem.Expect.That("").IsNotNull();

			Assert.Throws<ExpectException>(() => Totem.Expect.That<int?>(null).IsNotNull());
		}

		//
		// Equality
		//

		void Is()
		{
			Totem.Expect.That(1).Is(1);

			Assert.Throws<ExpectException>(() => Totem.Expect.That(1).Is(2));
		}

		void IsWithComparer()
		{
			Totem.Expect.That(1).Is(1, new IsComparer { Result = true });

			Assert.Throws<ExpectException>(() => Totem.Expect.That(1).Is(2, new IsComparer { Result = false }));
		}

		void IsNot()
		{
			Totem.Expect.That(1).IsNot(2);

			Assert.Throws<ExpectException>(() => Totem.Expect.That(1).IsNot(1));
		}

		void IsNotWithComparer()
		{
			Totem.Expect.That(1).IsNot(2, new IsComparer { Result = false });

			Assert.Throws<ExpectException>(() => Totem.Expect.That(1).IsNot(1, new IsComparer { Result = true }));
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
			Totem.Expect.That(true).IsTrue();

			Assert.Throws<ExpectException>(() => Totem.Expect.That(false).IsTrue());
		}

		void IsFalse()
		{
			Totem.Expect.That(false).IsFalse();

			Assert.Throws<ExpectException>(() => Totem.Expect.That(true).IsFalse());
		}

		//
		// Strings
		//

		void IsEmpty()
		{
			Totem.Expect.That("").IsEmpty();

			Assert.Throws<ExpectException>(() => Totem.Expect.That(" ").IsEmpty());
		}

		void IsNotEmpty()
		{
			Totem.Expect.That(" ").IsNotEmpty();

			Assert.Throws<ExpectException>(() => Totem.Expect.That("").IsNotEmpty());
		}
	}
}