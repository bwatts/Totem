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
		public static Expect<T> IsTrue<T>(
			this Expect<T> expect,
			Func<T, bool> check,
			Text issue = null,
			Text expected = null,
			Func<T, Text> received = null)
		{
			return expect.IsTrue(check, WriteMessage(check, issue, expected, received));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsFalse<T>(
			this Expect<T> expect,
			Func<T, bool> check,
			Text issue = null,
			Text expected = null,
			Func<T, Text> received = null)
		{
			return expect.IsFalse(check, WriteMessage(check, issue, expected, received));
		}

		private static Func<T, string> WriteMessage<T>(
			Func<T, bool> check,
			Text issue,
			Text expected,
			Func<T, Text> received)
		{
			if(issue == null)
			{
				issue = "Unexpected value";
			}

			if(received == null)
			{
				received = t => Text.Of(t);
			}

			return t =>
			{
				var message = issue.WriteTwoLines();

				if(expected != null)
				{
					message = message
						.WriteLine("Expected:")
						.WriteLine(expected.Indent())
						.WriteLine()
						.WriteLine("Received:");
				}
				else
				{
					message = message
						.WriteLine("Check:")
						.WriteLine(Text.Of("{0}.{1}", check.Method.DeclaringType, check.Method).Indent())
						.WriteLine()
						.WriteLine("Value:");
				}

				return message.WriteLine(received(t).Indent());
			};
		}

		//
		// Boolean
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<bool> IsTrue(this Expect<bool> expect, Text issue = null)
		{
			return expect.IsTrue(t => t, issue, Text.Of(true));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<bool> IsFalse(this Expect<bool> expect, Text issue = null)
		{
			return expect.IsFalse(t => t, issue, Text.Of(false));
		}

		//
		// Null
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNull<T>(this Expect<T> expect, Text issue = null) where T : class
		{
			return expect.IsTrue(t => t == null, issue, "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotNull<T>(this Expect<T> expect, Text issue = null) where T : class
		{
			return expect.IsTrue(t => t != null, issue, "not null", t => "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T?> IsNull<T>(this Expect<T?> expect, Text issue = null) where T : struct
		{
			return expect.IsTrue(t => t == null, issue, "null");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T?> IsNotNull<T>(this Expect<T?> expect, Text issue = null) where T : struct
		{
			return expect.IsTrue(t => t != null, issue, "not null", t => "null");
		}

		//
		// Equality
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> Is<T>(this Expect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Is(other, comparer), issue ?? "Values are not equal", Text.Of("{0} (comparer = {1})", other, comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> Is<T>(this Expect<T> expect, T other, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Is(other), issue ?? "Values are not equal", Text.Of(other));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNot<T>(this Expect<T> expect, T other, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsNot(other, comparer), issue ?? "Values are equal", Text.Of("not {0} (comparer = {1})", other, comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNot<T>(this Expect<T> expect, T other, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsNot(other), issue ?? "Values are equal", "not " + Text.Of(other));
		}

		//
		// Equality (in)
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, IEnumerable<T> values, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).IsIn(values),
				issue ?? "Value not found",
				expected: "in " + values.ToTextSeparatedBy(", ").InBrackets());
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, IEnumerable<T> values, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).IsIn(values),
				issue ?? "Value not found",
				expected: "in " + values.ToTextSeparatedBy(", ").InBrackets() + Text.Of(" (comparer = {0})", comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, params T[] values)
		{
			return expect.IsIn(values as IEnumerable<T>);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, Text issue, params T[] values)
		{
			return expect.IsIn(values as IEnumerable<T>, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, IEqualityComparer<T> comparer, params T[] values)
		{
			return expect.IsIn(values as IEnumerable<T>, comparer);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsIn<T>(this Expect<T> expect, IEqualityComparer<T> comparer, Text issue, params T[] values)
		{
			return expect.IsIn(values as IEnumerable<T>, comparer, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, IEnumerable<T> values, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).IsNotIn(values),
				issue ?? "Value found",
				expected: "not in " + values.ToTextSeparatedBy(", ").InBrackets());
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, IEnumerable<T> values, IEqualityComparer<T> comparer, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).IsNotIn(values),
				issue ?? "Value found",
				expected: "not in " + values.ToTextSeparatedBy(", ").InBrackets() + Text.Of(" (comparer = {0})", comparer));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, params T[] values)
		{
			return expect.IsNotIn(values as IEnumerable<T>);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, Text issue, params T[] values)
		{
			return expect.IsNotIn(values as IEnumerable<T>, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, IEqualityComparer<T> comparer, params T[] values)
		{
			return expect.IsNotIn(values as IEnumerable<T>, comparer);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsNotIn<T>(this Expect<T> expect, IEqualityComparer<T> comparer, Text issue, params T[] values)
		{
			return expect.IsNotIn(values as IEnumerable<T>, comparer, issue);
		}

    //
    // Comparison
    //
    
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> IsGreaterThan<T>(this Expect<T> expect, T other, Text issue = null) where T : IComparable<T>
    {
        return expect.IsTrue(t => Check.True(t).IsGreaterThan(other), issue ?? "greater than", "not greater than");
    }
    
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> IsGreaterThanOrEqualTo<T>(this Expect<T> expect, T other, Text issue = null) where T : IComparable<T>
    {
        return expect.IsTrue(t => Check.True(t).IsGreaterThanOrEqualTo(other), issue ?? "greater than or equal to", "not greater than or equal to");
    }
    
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> IsLessThan<T>(this Expect<T> expect, T other, Text issue = null) where T : IComparable<T>
    {
        return expect.IsTrue(t => Check.True(t).IsLessThan(other), issue ?? "less than", "not less than");
    }
    
    [DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
    public static Expect<T> IsLessThanOrEqualTo<T>(this Expect<T> expect, T other, Text issue = null) where T : IComparable<T>
    {
        return expect.IsTrue(t => Check.True(t).IsLessThanOrEqualTo(other), issue ?? "less than or equal to", "not less than or equal to");
    }

		//
		// String
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.IsTrue(t => t == "", issue, "empty");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNullOrEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.IsTrue(string.IsNullOrEmpty, issue, "null or empty");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNullOrWhiteSpace(this Expect<string> expect, Text issue = null)
		{
			return expect.IsTrue(string.IsNullOrWhiteSpace, issue, "null or whitespace");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNotEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.IsTrue(t => t != "", issue, "not empty", t => "empty");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNotNullOrEmpty(this Expect<string> expect, Text issue = null)
		{
			return expect.IsFalse(string.IsNullOrEmpty, issue, "not null or empty");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<string> IsNotNullOrWhiteSpace(this Expect<string> expect, Text issue = null)
		{
			return expect.IsFalse(string.IsNullOrWhiteSpace, issue, "not null or whitespace");
		}

		//
		// Int32
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<int> IsNegative(this Expect<int> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsNegative(), issue ?? "Integer out of range", expected: "negative");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<int> Is0OrNegative(this Expect<int> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Is0OrNegative(), issue ?? "Integer out of range", expected: "0 or negative");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<int> Is0(this Expect<int> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Is0(), issue ?? "Integer out of range", expected: "0");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<int> Is0OrPositive(this Expect<int> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Is0OrPositive(), issue ?? "Integer out of range", expected: "0 or positive");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<int> IsPositive(this Expect<int> expect, Text issue = null)
		{
			return expect.IsFalse(t => Check.True(t).IsPositive(), issue ?? "Integer out of range", expected: "positive");
		}

		//
		// Types
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<T> IsAssignableTo<T>(this Expect<T> expect, Type type, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsAssignableTo(type), issue ?? "Value is not assignable", "assignable to " + Text.Of(type));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Type> IsAssignableTo(this Expect<Type> expect, Type type, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsAssignableTo(type), issue ?? "Value is not assignable", "assignable to " + Text.Of(type));
		}

		//
		// Sequences
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has<T>(this Expect<Many<T>> expect, int count, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).Has(count),
				issue ?? "Wrong number of items",
				expected: Text.Count(count, "item"),
				received: t => Text.Count(t.Count, "item"));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> HasAtLeast<T>(this Expect<Many<T>> expect, int count, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).HasAtLeast(count),
				issue ?? "Too few items",
				expected: Text.Count(count, "item"),
				received: t => Text.Count(t.Count, "item"));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> HasAtMost<T>(this Expect<Many<T>> expect, int count, Text issue = null)
		{
			return expect.IsTrue(
				t => Check.True(t).HasAtMost(count),
				issue ?? "Too many items",
				expected: Text.Count(count, "item"),
				received: t => Text.Count(t.Count, "item"));
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has0<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.Has(0, issue ?? "No items");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has1<T>(this Expect<Many<T>> expect, Action<T> expectItem = null, Text issue = null)
		{
			expect = expect.Has(1, issue);

			if(expectItem != null)
			{
				expectItem(expect.Target[0]);
			}

			return expect;
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has2<T>(this Expect<Many<T>> expect, Action<T, T> expectItems = null, Text issue = null)
		{
			expect = expect.Has(2, issue);

			if(expectItems != null)
			{
				expectItems(expect.Target[0], expect.Target[1]);
			}

			return expect;
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has3<T>(this Expect<Many<T>> expect, Action<T, T, T> expectItems = null, Text issue = null)
		{
			expect = expect.Has(3, issue);

			if(expectItems != null)
			{
				expectItems(expect.Target[0], expect.Target[1], expect.Target[2]);
			}

			return expect;
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has4<T>(this Expect<Many<T>> expect, Action<T, T, T, T> expectItems = null, Text issue = null)
		{
			expect = expect.Has(4, issue);

			if(expectItems != null)
			{
				expectItems(expect.Target[0], expect.Target[1], expect.Target[2], expect.Target[3]);
			}

			return expect;
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has1OrMore<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Has1OrMore(), issue ?? "Too few items", expected: "1 or more items");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has2OrMore<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Has2OrMore(), issue ?? "Too few items", expected: "2 or more items");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has3OrMore<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Has3OrMore(), issue ?? "Too few items", expected: "3 or more items");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Many<T>> Has4OrMore<T>(this Expect<Many<T>> expect, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).Has4OrMore(), issue ?? "Too few items", expected: "4 or more items");
		}

		//
		// Tags
		//

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Field<T>> IsUnset<T>(this Expect<Field<T>> expect, IBindable target, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsUnset(target), issue, "tag not set", t => $"tag set: {t}");
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static Expect<Field<T>> IsSet<T>(this Expect<Field<T>> expect, IBindable target, Text issue = null)
		{
			return expect.IsTrue(t => Check.True(t).IsSet(target), issue, "tag set", t => $"tag set: {t}");
		}
	}
}