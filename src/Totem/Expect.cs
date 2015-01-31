using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="IExpect{T}"/> with expectations in specific scenarios
	/// </summary>
	public static class Expect
	{
		public static IExpect<T> That<T>(T value)
		{
			return new Expectation<T>(value);
		}

		private sealed class Expectation<T> : IExpect<T>
		{
			private readonly T _value;

			internal Expectation(T value)
			{
				_value = value;
			}

			public IExpect<T> IsTrue(Func<T, bool> check, Text expected, Text because = null, Func<T, Text> actual = null)
			{
				return Is(true, check, expected, because, actual);
			}

			public IExpect<T> IsFalse(Func<T, bool> check, Text expected, Text because = null, Func<T, Text> actual = null)
			{
				return Is(false, check, expected, because, actual);
			}

			private IExpect<T> Is(bool expectedResult, Func<T, bool> check, Text expected, Text because, Func<T, Text> actual)
			{
				if(check(_value) != expectedResult)
				{
					var actualText = actual == null ? Text.Of(_value) : actual(_value);

					throw new ExpectException(_value, expected, actualText, because);
				}

				return this;
			}
		}

		//
		// Null
		//

		public static IExpect<T> IsNull<T>(this IExpect<T> expect, Text because = null) where T : class
		{
			return expect.IsTrue(t => t == null, "null", because);
		}

		public static IExpect<T?> IsNull<T>(this IExpect<T?> expect, Text because = null) where T : struct
		{
			return expect.IsTrue(t => t == null, "null", because);
		}

		public static IExpect<T> IsNotNull<T>(this IExpect<T> expect, Text because = null) where T : class
		{
			return expect.IsTrue(t => t != null, "not null", because);
		}

		public static IExpect<T?> IsNotNull<T>(this IExpect<T?> expect, Text because = null) where T : struct
		{
			return expect.IsTrue(t => t != null, "not null", because);
		}

		//
		// Equality
		//

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text because = null)
		{
			return expect.IsTrue(
				t => comparer.Equals(t, other),
				Text.Of(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				because);
		}

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, Text because = null)
		{
			return expect.IsTrue(t => EqualityComparer<T>.Default.Equals(t, other), Text.Of(other), because);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text because = null)
		{
			return expect.IsFalse(
				t => comparer.Equals(t, other),
				Text.Of(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				because);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, Text because = null)
		{
			return expect.IsFalse(t => EqualityComparer<T>.Default.Equals(t, other), Text.Of(other), because);
		}

		//
		// String
		//

		public static IExpect<string> IsEmpty(this IExpect<string> expect, Text because = null)
		{
			return expect.IsTrue(x => x == "", because);
		}

		public static IExpect<string> IsNotEmpty(this IExpect<string> expect, Text because = null)
		{
			return expect.IsTrue(x => x != "", because);
		}

		//
		// Boolean
		//

		public static IExpect<bool> IsTrue(this IExpect<bool> expect, Text because = null)
		{
			return expect.IsTrue(t => t, Text.Of(true), because);
		}

		public static IExpect<bool> IsFalse(this IExpect<bool> expect, Text because = null)
		{
			return expect.IsFalse(t => t, Text.Of(false), because);
		}
	}
}