using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="Expect{T}"/> with expectations
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Expectable
	{
		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> That<T>(
			this Expect<T> expect,
			Func<Check<T>, bool> check,
			Text issue = null,
			Text expected = null,
			Func<T, Text> actual = null)
		{
			if(issue == null)
			{
				issue = "Unexpected value";
			}

			if(actual == null)
			{
				actual = t => Text.Of(t);
			}

			return expect.That(check, t =>
			{
				var message = issue.WriteTwoLines();

				if(expected != null)
				{
					message = message
						.WriteLine("Expected:")
						.WriteLine(expected.Indent())
						.WriteLine()
						.WriteLine("Actual:");
				}
				else
				{
					message = message
						.WriteLine("Check:")
						.WriteLine(Text.Of(check).Indent())
						.WriteLine()
						.WriteLine("Value:");
				}

				return message.WriteLine(actual(t).Indent());
			});
		}

		//
		// Boolean
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<bool> IsTrue(this Expect<bool> expect, Text issue = null)
		{
			return expect.That(t => t.IsTrue(), issue, Text.Of(true));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<bool> IsFalse(this Expect<bool> expect, Text issue = null)
		{
			return expect.That(t => t.IsFalse(), issue, Text.Of(false));
		}

		//
		// Null
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNull<T>(this Expect<T> expect, Text issue = null) where T : class
		{
			return expect.That(t => t.IsNull(), issue, "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotNull<T>(this Expect<T> expect, Text issue = null) where T : class
		{
			return expect.That(t => t.IsNotNull(), issue, "not null", t => "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T?> IsNull<T>(this Expect<T?> expect, Text issue = null) where T : struct
		{
			return expect.That(t => t.IsNull(), issue, "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T?> IsNotNull<T>(this Expect<T?> expect, Text issue = null) where T : struct
		{
			return expect.That(t => t.IsNotNull(), issue, "not null", t => "null");
		}

		//
		// Equality
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> Is<T>(this Expect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.That(t => t.Is(other, comparer), issue, Text.Of("{0} (comparer = {1})", other, comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> Is<T>(this Expect<T> expect, T other, Text issue = null)
		{
			return expect.That(t => t.Is(other), issue, Text.Of(other));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNot<T>(this Expect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.That(t => t.IsNot(other, comparer), issue, Text.Of("not {0} (comparer = {1})", other, comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNot<T>(this Expect<T> expect, T other, Text issue = null)
		{
			return expect.That(t => t.IsNot(other), issue, "not " + Text.Of(other));
		}

		//
		// String
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.That(t => t.IsEmpty(), issue, "empty");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNotEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.That(t => t.IsNotEmpty(), issue, "not empty", t => "empty");
		}

		//
		// Types
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsAssignableTo<T>(this Expect<T> expect, Type type, Text issue = null)
		{
			return expect.That(t => t.IsAssignableTo(type), issue, "assignable to " + Text.Of(type));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Type> IsAssignableTo(this Expect<Type> expect, Type type, Text issue = null)
		{
			return expect.That(t => t.IsAssignableTo(type), issue, "assignable to " + Text.Of(type));
		}

		//
		// Sequences
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has<T>(this Expect<Many<T>> expect, int count, Text issue = null)
		{
			return expect.That(t => t.Has(count), issue, Text.Count(count, "item"), t => Text.Count(t.Count, "item"));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has0<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.Has(0, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has1<T>(this Expect<Many<T>> expect, Action<T> itemExpect = null, Text issue = null)
		{
			expect = expect.Has(1, issue);

			if(itemExpect != null)
			{
				itemExpect(expect.Target[0]);
			}

			return expect;
		}
	}
}