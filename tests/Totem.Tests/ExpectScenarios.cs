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
			Expect<string>(null).IsNull();

			Assert.Throws<ExpectException>(() => Expect("").IsNull());
		}

		void IsNotNull()
		{
			Expect("").IsNotNull();

			Assert.Throws<ExpectException>(() => Expect<string>(null).IsNotNull());
		}

		void IsNullNullable()
		{
			Expect<int?>(null).IsNull();

			Assert.Throws<ExpectException>(() => Expect<int?>(0).IsNull());
		}

		void IsNotNullNullable()
		{
			Expect("").IsNotNull();

			Assert.Throws<ExpectException>(() => Expect<int?>(null).IsNotNull());
		}

		//
		// Equality
		//

		void Is()
		{
			Expect(1).Is(1);

			Assert.Throws<ExpectException>(() => Expect(1).Is(2));
		}

		void IsWithComparer()
		{
			Expect(1).Is(1, new IsComparer { Result = true });

			Assert.Throws<ExpectException>(() => Expect(1).Is(2, new IsComparer { Result = false }));
		}

		void IsNot()
		{
			Expect(1).IsNot(2);

			Assert.Throws<ExpectException>(() => Expect(1).IsNot(1));
		}

		void IsNotWithComparer()
		{
			Expect(1).IsNot(2, new IsComparer { Result = false });

			Assert.Throws<ExpectException>(() => Expect(1).IsNot(1, new IsComparer { Result = true }));
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
			Expect(true).IsTrue();

			Assert.Throws<ExpectException>(() => Expect(false).IsTrue());
		}

		void IsFalse()
		{
			Expect(false).IsFalse();

			Assert.Throws<ExpectException>(() => Expect(true).IsFalse());
		}

		//
		// Strings
		//

		void IsEmpty()
		{
			Expect("").IsEmpty();

			Assert.Throws<ExpectException>(() => Expect(" ").IsEmpty());
		}

		void IsNotEmpty()
		{
			Expect(" ").IsNotEmpty();

			Assert.Throws<ExpectException>(() => Expect("").IsNotEmpty());
		}
	}
}