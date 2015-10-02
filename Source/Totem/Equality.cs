using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Standard implementations of equality and comparison operations
	/// </summary>
	public static class Equality
	{
		public static Equatable<T> Check<T>(T x, T y)
		{
			return new Equatable<T>(x, y);
		}

		public static Comparable<T> Compare<T>(T x, T y)
		{
			return new Comparable<T>(x, y);
		}

		public static bool CheckOp<T>(T x, T y)
		{
			return EqualityComparer<T>.Default.Equals(x, y);
		}

		public static int CompareOp<T>(T x, T y)
		{
			return Comparer<T>.Default.Compare(x, y);
		}

		/// <summary>
		/// A value checked for equality with another value of the same type. Supports drilling down.
		/// </summary>
		/// <typeparam name="T">The type of equatable value</typeparam>
		public sealed class Equatable<T>
		{
			private readonly T _x;
			private readonly T _y;
			private bool _checked;
			private bool _result = true;

			internal Equatable(T x, T y)
			{
				_x = x;
				_y = y;
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

		/// <summary>
		/// A value compared to another value of the same type. Supports drilling down.
		/// </summary>
		/// <typeparam name="T">The type of comparable value</typeparam>
		public sealed class Comparable<T>
		{
			private readonly T _x;
			private readonly T _y;
			private bool _checked;
			private int _result;

			internal Comparable(T x, T y)
			{
				_x = x;
				_y = y;
			}

			public Comparable<T> Check<TValue>(Func<T, TValue> get)
			{
				if(_result == 0)
				{
					_result = Comparer<TValue>.Default.Compare(get(_x), get(_y));
				}

				_checked = true;

				return this;
			}

			public static implicit operator int(Comparable<T> comparable)
			{
				if(!comparable._checked)
				{
					throw new InvalidOperationException("Check at least one value to compare");
				}

				return comparable._result;
			}
		}
	}
}