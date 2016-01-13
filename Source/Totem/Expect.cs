using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="IExpect{T}"/> with expectations in specific scenarios
	/// </summary>
	public static class Expect
	{
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static IExpect<T> That<T>(T value)
		{
			return new Expectation<T>(value);
		}

		private sealed class Expectation<T> : IExpect<T>
		{
			private readonly T _value;

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      internal Expectation(T value)
			{
				_value = value;
			}

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      public IExpect<T> IsTrue(Func<T, bool> check, Text message = null)
			{
				return Is(true, check, message);
			}

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      public IExpect<T> IsFalse(Func<T, bool> check, Text message = null)
			{
				return Is(false, check, message);
			}

      [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
      private IExpect<T> Is(bool expectedResult, Func<T, bool> check, Text message)
			{
        bool result;

        try
        {
          result = check(_value);
        }
        catch(Exception error)
        {
          throw new ExpectException($"Check failed: {check}", error);
        }

        if(result != expectedResult)
        {
          throw new ExpectException(message ?? $"Expected {expectedResult}, actual {result}: {check}");
        }

				return this;
			}
		}

    //
    // Throws
    //

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws<TException>(Action action, Text message = null) where TException : Exception
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
        throw new ExpectException(message ?? $"Expected {Text.OfType<TException>()}, actual: {error.GetType()}", error);
			}
		}

    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static void Throws<TException>(Func<object> func, Text issue = null) where TException : Exception
		{
			Throws<TException>(() => { func(); });
		}

		//
		// Null
		//

		public static IExpect<T> IsNull<T>(this IExpect<T> expect, Text message = null)
      where T : class
		{
      return expect.IsTrue(t => t == null, message ?? "Expected null");
		}

		public static IExpect<T?> IsNull<T>(this IExpect<T?> expect, Text message = null)
			where T : struct
		{
			return expect.IsTrue(t => t == null, message ?? "Expected null");
		}

		public static IExpect<T> IsNotNull<T>(this IExpect<T> expect, Text message = null)
			where T : class
		{
      return expect.IsTrue(t => t != null, message ?? "Expected not null");
    }

		public static IExpect<T?> IsNotNull<T>(this IExpect<T?> expect, Text message = null)
			where T : struct
		{
      return expect.IsTrue(t => t != null, message ?? "Expected not null");
    }

		//
		// Equality
		//

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text message = null)
		{
      return expect.IsTrue(t => comparer.Equals(t, other), message ?? "Values are not equal");
		}

		public static IExpect<T> Is<T>(this IExpect<T> expect, T other, Text message = null)
		{
      return expect.Is(other, EqualityComparer<T>.Default, message);
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, IEqualityComparer<T> comparer, Text message = null)
		{
      return expect.IsFalse(t => comparer.Equals(t, other), message ?? "Expected valeus are not equal");
		}

		public static IExpect<T> IsNot<T>(this IExpect<T> expect, T other, Text message = null)
		{
      return expect.IsNot(other, EqualityComparer<T>.Default, message);
		}

		//
		// String
		//

		public static IExpect<string> IsEmpty(this IExpect<string> expect, Text message = null)
		{
			return expect.IsTrue(t => t == "", message ?? "Expected empty string");
		}

		public static IExpect<string> IsNotEmpty(this IExpect<string> expect, Text message = null)
		{
			return expect.IsTrue(t => t != "", message ?? "Expected non-empty string");
		}

		//
		// Boolean
		//

		public static IExpect<bool> IsTrue(this IExpect<bool> expect, Text message = null)
		{
			return expect.IsTrue(t => t, message ?? "Expected true");
		}

		public static IExpect<bool> IsFalse(this IExpect<bool> expect, Text message = null)
		{
			return expect.IsFalse(t => t, message ?? "Expected false");
		}

		//
		// Types
		//

		public static IExpect<T> IsAssignableTo<T>(this IExpect<T> expect, Type type, Text message = null)
		{
      return expect.IsTrue(t => type.IsAssignableFrom(t.GetType()), message ?? "Expected assignable value");
		}

		public static IExpect<Type> IsAssignableTo(this IExpect<Type> expect, Type type, Text message = null)
		{
      return expect.IsTrue(type.IsAssignableFrom, message ?? "Expected assignable value");
		}
	}
}