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

			public IExpect<T> IsTrue(Func<T, bool> check, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			{
				return Is(true, check, issue, expected, actual);
			}

			public IExpect<T> IsFalse(Func<T, bool> check, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			{
				return Is(false, check, issue, expected, actual);
			}

			private IExpect<T> Is(bool expectedResult, Func<T, bool> check, Text issue, Text expected, Func<T, Text> actual)
			{
				if(check(_value) != expectedResult)
				{
					var issueString = issue != null ? issue.ToString() : "Unexpected value";
					var expectedString = (string) expected;
					var actualString = Text.If(expectedString == "", "", actual != null ? actual(_value) : Text.Of(_value));

					throw new ExpectException(issueString, expectedString, actualString);
				}

				return this;
			}
		}

		//
		// Conditions
		//

		public static void True(bool condition, Text issue = null, Text expected = null, Text actual = null)
		{
			Expect.That(condition).IsTrue(t => t, issue, expected, t => actual ?? Text.None);
		}

		public static void False(bool condition, Text issue = null, Text expected = null, Text actual = null)
		{
			Expect.That(condition).IsFalse(t => t, issue, expected, t => actual ?? Text.None);
		}

		public static void Throws<TException>(Action action, Text issue = null) where TException : Exception
		{
			try
			{
				action();
			}
			catch(TException)
			{
				return;
			}
			catch(Exception error)
			{
				throw new ExpectException(
					error,
					issue,
					expected: "An exception of type " + Text.OfType<TException>(),
					actual: "An unexpected exception");
			}

			throw new ExpectException(
				issue,
				expected: "An exception of type " + Text.OfType<TException>(),
				actual: "No exception");
		}

		public static void Throws<TException>(Func<object> func, Text issue = null) where TException : Exception
		{
			Totem.Expect.Throws<TException>(() => { func(); });
		}

		//
		// Null
		//

		public static IExpect<T> IsNull<T>(this IExpect<T> expect, Text issue = null, Func<T, Text> actual = null)
			where T : class
		{
			return expect.IsTrue(t => t == null, issue, "null", actual);
		}

		public static IExpect<T?> IsNull<T>(this IExpect<T?> expect, Text issue = null, Func<T?, Text> actual = null)
			where T : struct
		{
			return expect.IsTrue(t => t == null, issue, "null", actual);
		}

		public static IExpect<T> IsNotNull<T>(this IExpect<T> expect, Text issue = null, Func<T, Text> actual = null)
			where T : class
		{
			return expect.IsTrue(t => t != null, issue, "not null", actual);
		}

		public static IExpect<T?> IsNotNull<T>(this IExpect<T?> expect, Text issue = null, Func<T?, Text> actual = null)
			where T : struct
		{
			return expect.IsTrue(t => t != null, issue, "not null", actual);
		}

		//
		// Equality
		//

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => comparer.Equals(t, other),
				issue,
				expected: Text.Of(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				actual: actual);
		}

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, Text issue = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => EqualityComparer<T>.Default.Equals(t, other),
				issue,
				expected: Text.Of(other),
				actual: actual);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null, Func<T, Text> actual = null)
		{
			return expect.IsFalse(
				t => comparer.Equals(t, other),
				issue,
				expected: Text.Of("not ").Write(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				actual: actual);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, Text issue = null, Func<T, Text> actual = null)
		{
			return expect.IsFalse(
				t => EqualityComparer<T>.Default.Equals(t, other),
				issue,
				expected: Text.Of("not ").Write(other),
				actual: actual);
		}

		//
		// String
		//

		public static IExpect<string> IsEmpty(this IExpect<string> expect, Text issue = null, Func<string, Text> actual = null)
		{
			return expect.IsTrue(t => t == "", issue, "empty", actual);
		}

		public static IExpect<string> IsNotEmpty(this IExpect<string> expect, Text issue = null, Func<string, Text> actual = null)
		{
			return expect.IsTrue(t => t != "", issue, "not empty", actual);
		}

		//
		// Boolean
		//

		public static IExpect<bool> IsTrue(this IExpect<bool> expect, Text issue = null, Func<bool, Text> actual = null)
		{
			return expect.IsTrue(t => t, issue, Text.Of(true), actual);
		}

		public static IExpect<bool> IsFalse(this IExpect<bool> expect, Text issue = null, Func<bool, Text> actual = null)
		{
			return expect.IsFalse(t => t, issue, Text.Of(false), actual);
		}

		//
		// Types
		//

		public static IExpect<T> IsAssignableTo<T>(this IExpect<T> expect, Type type, Text issue = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => type.IsAssignableFrom(t.GetType()),
				issue,
				expected: "Assignable to " + Text.OfType(type),
				actual: actual ?? (t => Text.Of(t)));
		}

		public static IExpect<Type> IsAssignableTo(this IExpect<Type> expect, Type type, Text issue = null, Func<Type, Text> actual = null)
		{
			return expect.IsTrue(
				type.IsAssignableFrom,
				issue,
				expected: "Assignable to " + Text.OfType(type),
				actual: actual ?? (t => Text.Of(t)));
		}
	}
}