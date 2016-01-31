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
		public static Expect<T> That<T>(T target)
		{
			return new Expect<T>(target);
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
				var message = Text
					.Of("Unexpected exception type")
					.WriteTwoLines()
					.WriteLine("Expected:")
					.WriteLine(Text.OfType<TException>().Indent(retainWhitespace: true))
					.WriteLine()
					.WriteLine("Actual:")
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
		internal Expect(T target)
		{
			Target = target;
		}

		public T Target { get; }

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public Expect<T> That(Func<Check<T>, bool> check, Func<T, string> message)
		{
			bool result;

			try
			{
				result = check(Check.That(Target));
			}
			catch(Exception ex)
			{
				throw new ExpectException(message(Target), ex);
			}

			if(!result)
			{
				throw new ExpectException(message(Target));
			}

			return this;
		}
	}
}