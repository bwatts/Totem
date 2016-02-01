using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="Spec{T}"/> with expectations
	/// </summary>
	public static class Expect
	{
		public static Expect<T> True<T>(T target)
		{
			return new Expect<T>(target, false);
		}

		public static Expect<T> False<T>(T target)
		{
			return new Expect<T>(target, false);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void True(bool result)
		{
			True<bool>(result).IsTrue(t => t);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void True(bool result, Text issue)
		{
			True<bool>(result).IsTrue(issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void False(bool result)
		{
			True<bool>(result).IsFalse();
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void False(bool result, Text issue)
		{
			True<bool>(result).IsFalse(issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
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
				var message = (issue ?? "Unexpected exception type")
					.WriteTwoLines()
					.WriteLine("Expected:")
					.WriteLine(Text.OfType<TException>().Indent(retainWhitespace: true))
					.WriteLine()
					.WriteLine("Received:")
					.WriteLine(Text.OfType(error).Indent(retainWhitespace: true));

				throw new ExpectException(message, error);
			}
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void Throws<TException>(Func<object> func, Text issue = null) where TException : Exception
		{
			Throws<TException>(() => { func(); }, issue);
		}
	}

	/// <summary>
	/// An assertable value of the specified type
	/// </summary>
	/// <typeparam name="T">The type of target value</typeparam>
	public sealed class Expect<T>
	{
		private readonly bool _expectedResult;

		internal Expect(T target, bool expectedResult)
		{
			Target = target;
			_expectedResult = expectedResult;
		}

		public T Target { get; }

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public Expect<T> IsTrue(Func<T, bool> check, Func<T, string> message)
		{
			return Is(true, check, message);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public Expect<T> IsFalse(Func<T, bool> check, Func<T, string> message)
		{
			return Is(false, check, message);
		}

		private Expect<T> Is(bool expectedResult, Func<T, bool> check, Func<T, string> message)
		{
			bool result;

			try
			{
				result = check(Target);
			}
			catch(Exception ex)
			{
				throw new ExpectException(message(Target), ex);
			}

			if(result != expectedResult)
			{
				throw new ExpectException(message(Target));
			}

			return this;
		}
	}
}