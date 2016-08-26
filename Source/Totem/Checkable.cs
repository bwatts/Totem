﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="Check{T}"/> with checks in specific scenarios
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Checkable
	{
		//
		// Boolean
		//

		public static Check<bool> IsTrue(this Check<bool> check)
		{
			return check.IsTrue(t => t);
		}

		public static Check<bool> IsFalse(this Check<bool> check)
		{
			return check.IsTrue(t => !t);
		}

		//
		// Null
		//

		public static Check<T> IsNull<T>(this Check<T> check) where T : class
		{
			return check.IsTrue(t => t == null);
		}

		public static Check<T?> IsNull<T>(this Check<T?> check) where T : struct
		{
			return check.IsTrue(t => t == null);
		}

		public static Check<T> IsNotNull<T>(this Check<T> check) where T : class
		{
			return check.IsTrue(t => t != null);
		}

		public static Check<T?> IsNotNull<T>(this Check<T?> check) where T : struct
		{
			return check.IsTrue(t => t != null);
		}

		//
		// Equality
		//

		public static Check<T> Is<T>(this Check<T> check, T other, IEqualityComparer<T> comparer)
		{
			return check.IsTrue(t => comparer.Equals(t, other));
		}

		public static Check<T> Is<T>(this Check<T> check, T other)
		{
			return check.Is(other, EqualityComparer<T>.Default);
		}

		public static Check<T> IsNot<T>(this Check<T> check, T other, IEqualityComparer<T> comparer)
		{
			return check.IsFalse(t => comparer.Equals(t, other));
		}

		public static Check<T> IsNot<T>(this Check<T> check, T other)
		{
			return check.IsNot(other, EqualityComparer<T>.Default);
		}

//
		// Equality (in)
		//

		public static Check<T> IsIn<T>(this Check<T> check, IEnumerable<T> values)
		{
			return check.IsTrue(values.Contains);
		}

		public static Check<T> IsIn<T>(this Check<T> check, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			return check.IsTrue(t => values.Contains(t, comparer));
		}

		public static Check<T> IsIn<T>(this Check<T> check, params T[] values)
		{
			return check.IsIn(values as IEnumerable<T>);
		}

		public static Check<T> IsIn<T>(this Check<T> check, IEqualityComparer<T> comparer, params T[] values)
		{
			return check.IsIn(values as IEnumerable<T>, comparer);
		}

		public static Check<T> IsNotIn<T>(this Check<T> check, IEnumerable<T> values)
		{
			return check.IsFalse(values.Contains);
		}

		public static Check<T> IsNotIn<T>(this Check<T> check, IEnumerable<T> values, IEqualityComparer<T> comparer)
		{
			return check.IsFalse(t => values.Contains(t, comparer));
		}

		public static Check<T> IsNotIn<T>(this Check<T> check, params T[] values)
		{
			return check.IsNotIn(values as IEnumerable<T>);
		}

		public static Check<T> IsNotIn<T>(this Check<T> check, IEqualityComparer<T> comparer, params T[] values)
		{
			return check.IsNotIn(values as IEnumerable<T>, comparer);
		}
    
    //
    // Comparison
    //
    
    public static Check<T> IsGreaterThan<T>(this Check<T> check, T other) where T : IComparable<T>
    {
      return check.IsTrue(t => t.CompareTo(other) > 0);
    }
    
    public static Check<T> IsGreaterThanOrEqualTo<T>(this Check<T> check, T other) where T : IComparable<T>
    {
      return check.IsTrue(t => t.CompareTo(other) >= 0);
    }
    
    public static Check<T> IsLessThan<T>(this Check<T> check, T other) where T : IComparable<T>
    {
      return check.IsTrue(t => t.CompareTo(other) < 0);
    }
    
    public static Check<T> IsLessThanOrEqualTo<T>(this Check<T> check, T other) where T : IComparable<T>
    {
      return check.IsTrue(t => t.CompareTo(other) <= 0);
    }

		//
		// String
		//

		public static Check<string> IsEmpty(this Check<string> check)
		{
			return check.IsTrue(t => t == "");
		}

		public static Check<string> IsNullOrEmpty(this Check<string> check)
		{
			return check.IsTrue(string.IsNullOrEmpty);
		}

		public static Check<string> IsNullOrWhiteSpace(this Check<string> check)
		{
			return check.IsTrue(string.IsNullOrWhiteSpace);
		}

		public static Check<string> IsNotEmpty(this Check<string> check)
		{
			return check.IsTrue(t => t != "");
		}

		public static Check<string> IsNotNullOrEmpty(this Check<string> check)
		{
			return check.IsFalse(string.IsNullOrWhiteSpace);
		}

		public static Check<string> IsNotNullOrWhiteSpace(this Check<string> check)
		{
			return check.IsFalse(string.IsNullOrWhiteSpace);
		}

		//
		// Int32
		//

		public static Check<int> IsNegative(this Check<int> check)
		{
			return check.IsTrue(t => t < 0);
		}

		public static Check<int> Is0OrNegative(this Check<int> check)
		{
			return check.IsTrue(t => t <= 0);
		}

		public static Check<int> Is0(this Check<int> check)
		{
			return check.IsTrue(t => t == 0);
		}

		public static Check<int> Is0OrPositive(this Check<int> check)
		{
			return check.IsTrue(t => t >= 0);
		}

		public static Check<int> IsPositive(this Check<int> check)
		{
			return check.IsTrue(t => t > 0);
		}

		//
		// Types
		//

		public static Check<T> IsAssignableTo<T>(this Check<T> check, Type type)
		{
			return check.IsTrue(t => type.IsAssignableFrom(t.GetType()));
		}

		public static Check<Type> IsAssignableTo(this Check<Type> check, Type type)
		{
			return check.IsTrue(type.IsAssignableFrom);
		}

		//
		// Sequences
		//

		public static Check<Many<T>> Has<T>(this Check<Many<T>> check, int count)
		{
			return check.IsTrue(t => t.Count == count);
		}

		public static Check<Many<T>> HasAtLeast<T>(this Check<Many<T>> check, int count)
		{
			return check.IsTrue(t => t.Count >= count);
		}

		public static Check<Many<T>> HasAtMost<T>(this Check<Many<T>> check, int count)
		{
			return check.IsTrue(t => t.Count <= count);
		}

		public static Check<Many<T>> Has0<T>(this Check<Many<T>> check)
		{
			return check.Has(0);
		}

		public static Check<Many<T>> Has1<T>(this Check<Many<T>> check, Func<T, bool> checkItem = null)
		{
			check = check.Has(1);

			if(checkItem != null)
			{
				check = check.IsTrue(t => checkItem(t[0]));
			}

			return check;
		}

		public static Check<Many<T>> Has2<T>(this Check<Many<T>> check, Func<T, T, bool> checkItems = null)
		{
			check = check.Has(2);

			if(checkItems != null)
			{
				check = check.IsTrue(t => checkItems(t[0], t[1]));
			}

			return check;
		}

		public static Check<Many<T>> Has3<T>(this Check<Many<T>> check, Func<T, T, T, bool> checkItems = null)
		{
			check = check.Has(3);

			if(checkItems != null)
			{
				check = check.IsTrue(t => checkItems(t[0], t[1], t[2]));
			}

			return check;
		}

		public static Check<Many<T>> Has4<T>(this Check<Many<T>> check, Func<T, T, T, T, bool> checkItems = null)
		{
			check = check.Has(2);

			if(checkItems != null)
			{
				check = check.IsTrue(t => checkItems(t[0], t[1], t[2], t[3]));
			}

			return check;
		}

		public static Check<Many<T>> Has1OrMore<T>(this Check<Many<T>> check)
		{
			return check.HasAtLeast(1);
		}

		public static Check<Many<T>> Has2OrMore<T>(this Check<Many<T>> check)
		{
			return check.HasAtLeast(2);
		}

		public static Check<Many<T>> Has3OrMore<T>(this Check<Many<T>> check)
		{
			return check.HasAtLeast(3);
		}

		public static Check<Many<T>> Has4OrMore<T>(this Check<Many<T>> check)
		{
			return check.HasAtLeast(4);
		}

		//
		// Tags
		//

		public static Check<Tag<T>> IsUnset<T>(this Check<Tag<T>> check, ITaggable target)
		{
			return check.IsTrue(t => t.IsUnset(target));
		}

		public static Check<Tag<T>> IsSet<T>(this Check<Tag<T>> check, ITaggable target)
		{
			return check.IsTrue(t => t.IsSet(target));
		}
	}
}