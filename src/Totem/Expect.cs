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
					throw new ExpectException(
						issue != null ? issue.ToString() : "Unexpected value",
						expected,
						actual != null ? actual(_value) : Text.Of(_value));
				}

				return this;
			}
		}

		//
		// Conditions
		//

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
					issue ?? "Unexpected exception",
					expected: "An exception of type " + Text.OfType<TException>(),
					actual: "An unexpected exception");
			}

			throw new ExpectException(
				issue ?? "Exception did not occur",
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

		public static IExpect<T> IsNull<T>(this IExpect<T> expect, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			where T : class
		{
			return expect.IsTrue(t => t == null, issue ?? "Value is not null", expected ?? "null", actual);
		}

		public static IExpect<T?> IsNull<T>(this IExpect<T?> expect, Text issue = null, Text expected = null, Func<T?, Text> actual = null)
			where T : struct
		{
			return expect.IsTrue(t => t == null, issue ?? "Value is not null", expected ?? "null", actual);
		}

		public static IExpect<T> IsNotNull<T>(this IExpect<T> expect, Text issue = null, Text expected = null, Func<T, Text> actual = null)
			where T : class
		{
			return expect.IsTrue(t => t != null, issue ?? "Value is null", expected ?? "not null", actual);
		}

		public static IExpect<T?> IsNotNull<T>(this IExpect<T?> expect, Text issue = null, Text expected = null, Func<T?, Text> actual = null)
			where T : struct
		{
			return expect.IsTrue(t => t != null, issue ?? "Value is null", expected ?? "not null", actual);
		}

		//
		// Equality
		//

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null, Text expected = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => comparer.Equals(t, other),
				issue ?? "Value does not equal other value",
				expected ?? Text.Of(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				actual);
		}

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, Text issue = null, Text expected = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => EqualityComparer<T>.Default.Equals(t, other),
				issue ?? "Value does not equal other value",
				expected ?? Text.Of(other),
				actual);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null, Text expected = null, Func<T, Text> actual = null)
		{
			return expect.IsFalse(
				t => comparer.Equals(t, other),
				issue ?? "Value equals other value",
				expected ?? Text.Of("not ").Write(other).Write(" ").WriteInParentheses(Text.Of(comparer)),
				actual);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, Text issue = null, Text expected = null, Func<T, Text> actual = null)
		{
			return expect.IsFalse(
				t => EqualityComparer<T>.Default.Equals(t, other),
				issue ?? "Value equals other value",
				expected ?? Text.Of("not ").Write(other),
				actual);
		}

		//
		// String
		//

		public static IExpect<string> IsEmpty(this IExpect<string> expect, Text issue = null, Text expected = null, Func<string, Text> actual = null)
		{
			return expect.IsTrue(t => t == "", issue ?? "String is not empty", "empty", actual);
		}

		public static IExpect<string> IsNotEmpty(this IExpect<string> expect, Text issue = null, Text expected = null, Func<string, Text> actual = null)
		{
			return expect.IsTrue(t => t != "", issue ?? "String is empty", "not empty", actual);
		}

		//
		// Boolean
		//

		public static IExpect<bool> IsTrue(this IExpect<bool> expect, Text issue = null, Text expected = null, Func<bool, Text> actual = null)
		{
			return expect.IsTrue(t => t, issue ?? "Value is not true", Text.Of(true), actual);
		}

		public static IExpect<bool> IsFalse(this IExpect<bool> expect, Text issue = null, Text expected = null, Func<bool, Text> actual = null)
		{
			return expect.IsFalse(t => t, issue ?? "Value is not false", Text.Of(false), actual);
		}

		//
		// Types
		//

		public static IExpect<T> IsAssignableTo<T>(this IExpect<T> expect, Type type, Text issue = null, Text expected = null, Func<T, Text> actual = null)
		{
			return expect.IsTrue(
				t => type.IsAssignableFrom(t.GetType()),
				issue ?? "Value is not assignable to type",
				expected ?? Text.OfType(type),
				actual);
		}

		public static IExpect<Type> IsAssignableTo(this IExpect<Type> expect, Type type, Text issue = null, Text expected = null, Func<Type, Text> actual = null)
		{
			return expect.IsTrue(
				type.IsAssignableFrom,
				issue ?? "Type is not assignable to type",
				expected ?? Text.OfType(type),
				actual);
		}
	}
}