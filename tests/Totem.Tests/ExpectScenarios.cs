using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			Expect.That<string>(null).IsNull();

			Assert.Throws<ExpectException>(() => Expect.That("").IsNull());
		}

		void IsNotNull()
		{
			Expect.That("").IsNotNull();

			Assert.Throws<ExpectException>(() => Expect.That<string>(null).IsNotNull());
		}

		void IsNullNullable()
		{
			Expect.That<int?>(null).IsNull();

			Assert.Throws<ExpectException>(() => Expect.That<int?>(0).IsNull());
		}

		void IsNotNullNullable()
		{
			Expect.That("").IsNotNull();

			Assert.Throws<ExpectException>(() => Expect.That<int?>(null).IsNotNull());
		}

		//
		// Equality
		//

		void Is()
		{
			Expect.That(1).Is(1);

			Assert.Throws<ExpectException>(() => Expect.That(1).Is(2));
		}

		void IsWithComparer()
		{
			Expect.That(1).Is(1, new IsComparer { Result = true });

			Assert.Throws<ExpectException>(() => Expect.That(1).Is(2, new IsComparer { Result = false }));
		}

		void IsNot()
		{
			Expect.That(1).IsNot(2);

			Assert.Throws<ExpectException>(() => Expect.That(1).IsNot(1));
		}

		void IsNotWithComparer()
		{
			Expect.That(1).IsNot(2, new IsComparer { Result = false });

			Assert.Throws<ExpectException>(() => Expect.That(1).IsNot(1, new IsComparer { Result = true }));
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
			Expect.That(true).IsTrue();

			Assert.Throws<ExpectException>(() => Expect.That(false).IsTrue());
		}

		void IsFalse()
		{
			Expect.That(false).IsFalse();

			Assert.Throws<ExpectException>(() => Expect.That(true).IsFalse());
		}

		//
		// Strings
		//

		void IsEmpty()
		{
			Expect.That("").IsEmpty();

			Assert.Throws<ExpectException>(() => Expect.That(" ").IsEmpty());
		}

		void IsNotEmpty()
		{
			Expect.That(" ").IsNotEmpty();

			Assert.Throws<ExpectException>(() => Expect.That("").IsNotEmpty());
		}
	}
}