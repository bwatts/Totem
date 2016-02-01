using System;
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
		// String
		//

		public static Check<string> IsEmpty(this Check<string> check)
		{
			return check.IsTrue(t => t == "");
		}

		public static Check<string> IsNotEmpty(this Check<string> check)
		{
			return check.IsTrue(t => t != "");
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

		//
		// Tags
		//

		public static Check<Tag<T>> IsUnset<T>(this Check<Tag<T>> check, ITaggable target)
		{
			return check.IsTrue(t => t.IsUnset(target));
		}

		public static Check<Tag<T>> IsSet<T>(this Check<Tag<T>> check, ITaggable target)
		{
			return check.IsFalse(t => t.IsUnset(target));
		}
	}
}