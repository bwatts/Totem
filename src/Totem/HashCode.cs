using System.Collections.Generic;

namespace Totem
{
  /// <summary>
  /// Combines the hash codes of multiple objects
  /// </summary>
  public static class HashCode
  {
    public static int Combine(params object[] source) =>
      CombineItems(source as IEnumerable<object>);

    public static int CombineItems<T>(IEnumerable<T> items)
    {
      int hash;

      if(items is IList<T> list && list.Count == 1)
      {
        hash = list[0].GetHashCode();
      }
      else
      {
        // Based on: http://blog.roblevine.co.uk/?cat=10

        hash = 23;	// Non-zero and prime

        foreach(var item in items)
        {
          hash = ((hash << 5) * 37) ^ (item == null ? 0 : item.GetHashCode());
        }
      }

      return hash;
    }
  }
}