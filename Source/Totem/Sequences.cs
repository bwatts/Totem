using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends sequences with various capabilities
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Sequences
	{
		//
		// ToMany
		//

		public static Many<T> ToMany<T>(this IEnumerable<T> source)
		{
			return Many.Of(source);
		}

		public static Many<TResult> ToMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selectItem)
		{
			return Many.Of(source.Select(selectItem));
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, IEnumerable<T> itemsAfter)
		{
			return Many.Of(itemsBefore, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0)
		{
			return Many.Of(itemsBefore, item0);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, IEnumerable<T> itemsAfter)
		{
			return Many.Of(itemsBefore, item0, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1)
		{
			return Many.Of(itemsBefore, item0, item1);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, IEnumerable<T> itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2)
		{
			return Many.Of(itemsBefore, item0, item1, item2);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2, IEnumerable<T> itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3)
		{
			return Many.Of(itemsBefore, item0, item1, item2, item3);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, IEnumerable<T> itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, item3, itemsAfter);
		}

		public static Many<T> ToMany<T>(this IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, item3, itemsAfter);
		}

		//
		// ToHashSet
		//

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
		{
			return new HashSet<T>(items, comparer);
		}

		public static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selectItem)
		{
			return items.Select(selectItem).ToHashSet();
		}

		public static HashSet<TResult> ToHashSet<TSource, TResult>(this IEnumerable<TSource> items, Func<TSource, TResult> selectItem, IEqualityComparer<TResult> comparer)
		{
			return items.Select(selectItem).ToHashSet(comparer);
		}

		//
		// ToDictionary
		//

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
		{
			return new Dictionary<TKey, TValue>(source);
		}

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source, IEqualityComparer<TKey> comparer)
		{
			return new Dictionary<TKey, TValue>(source, comparer);
		}

		//
		// Reads
		//

		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items)
    {
      return source.Except(items as IEnumerable<T>);
    }

		//
		// Writes
		//

		public static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
		{
			foreach(var item in items)
			{
				source.Add(item);
			}
		}

		public static void AddRange<T>(this IList<T> source, params T[] items)
		{
			source.AddRange(items as IEnumerable<T>);
		}

		public static void AddRange<T>(this ISet<T> set, IEnumerable<T> newItems)
		{
			foreach(var newItem in newItems)
			{
				set.Add(newItem);
			}
		}

		public static void AddRange<T>(this ISet<T> set, params T[] newItems)
		{
			set.AddRange(newItems as IEnumerable<T>);
		}

		public static void Replace<T>(this IList<T> list, IEnumerable<T> newItems)
		{
			newItems = newItems.ToList();

			list.Clear();

			foreach(var newItem in newItems)
			{
				list.Add(newItem);
			}
		}

		public static void Replace<T>(this IList<T> list, params T[] newItems)
		{
			list.Replace(newItems as IEnumerable<T>);
		}

    public static void Reorder<T, TKey>(this IList<T> list, Func<T, TKey> keySelector)
    {
      list.Replace(list.OrderBy(keySelector));
    }

    public static void Reorder<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
      list.Replace(list.OrderBy(keySelector, comparer));
    }

    public static void ReorderDescending<T, TKey>(this IList<T> list, Func<T, TKey> keySelector)
    {
      list.Replace(list.OrderByDescending(keySelector));
    }

    public static void ReorderDescending<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, IComparer<TKey> comparer)
    {
      list.Replace(list.OrderByDescending(keySelector, comparer));
    }

    public static bool RemoveFirst<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for(var i = 0; i < list.Count; i++)
			{
				if(predicate(list[i]))
				{
					list.RemoveAt(i);

					return true;
				}
			}

			return false;
		}

    public static List<T> RemoveAll<T>(this ICollection<T> collection)
    {
      var items = collection.ToList();

      collection.Clear();

      return items;
    }
  }
}