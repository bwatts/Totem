using System;
using System.Collections.Generic;

namespace Totem
{
  /// <summary>
  /// Standard implementations of equality and comparison operations
  /// </summary>
  public static class Cmp
  {
    public static Comparable<T> Values<T>(T x, T y) =>
      new Comparable<T>(x, y);

    public static int Op<T>(T x, T y) =>
      Comparer<T>.Default.Compare(x, y);

    /// <summary>
    /// A value compared with another value of the same type. Supports drilldown.
    /// </summary>
    /// <typeparam name="T">The type of comparable value</typeparam>
    public struct Comparable<T>
    {
      readonly T _x;
      readonly T _y;
      bool _checked;
      int _result;

      internal Comparable(T x, T y) : this()
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
          throw new InvalidOperationException("Check at least one value for comparison");
        }

        return comparable._result;
      }
    }
  }
}