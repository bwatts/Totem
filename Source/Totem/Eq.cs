using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Standard implementations of equality operations
	/// </summary>
	public static class Eq
	{
		public static Equatable<T> Values<T>(T x, T y)
		{
			return new Equatable<T>(x, y);
		}

		public static bool Op<T>(T x, T y)
		{
			return EqualityComparer<T>.Default.Equals(x, y);
		}

		public static bool OpNot<T>(T x, T y)
		{
			return !Op(x, y);
		}

		/// <summary>
		/// A value checked for equality with another value of the same type. Supports drilldown.
		/// </summary>
		/// <typeparam name="T">The type of equatable value</typeparam>
		/// <remarks>The null check causes boxing - revist to reduce allocations</remarks>
		public sealed class Equatable<T>
		{
			private readonly T _x;
			private readonly T _y;
			private readonly bool _yNull;
			private bool _checked;
			private bool _result;

			internal Equatable(T x, T y)
			{
				_x = x;
				_y = y;
				_result = !ReferenceEquals(y, null);
			}

			public Equatable<T> Check<TValue>(Func<T, TValue> get)
			{
				_result = _result && EqualityComparer<TValue>.Default.Equals(get(_x), get(_y));

				_checked = true;

				return this;
			}

			public static implicit operator bool(Equatable<T> equatable)
			{
				if(!equatable._checked)
				{
					throw new InvalidOperationException("Check at least one value for equality");
				}

				return equatable._result;
			}
		}
	}
}