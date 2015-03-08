using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Extends sequences with various useful operations
	/// </summary>
	public static class Many
	{
		public static List<T> Of<T>()
		{
			return new List<T>();
		}

		public static List<T> Of<T>(params T[] items)
		{
			return items.ToList();
		}

		//
		// Item 0
		//

		public static List<T> Of<T>(T item0)
		{
			return new List<T> { item0 };
		}

		public static List<T> Of<T>(T item0, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(item0);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(T item0, params T[] itemsAfter)
		{
			return Many.Of(item0, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 1
		//

		public static List<T> Of<T>(T item0, T item1)
		{
			return new List<T> { item0, item1 };
		}

		public static List<T> Of<T>(T item0, T item1, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(item0, item1);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(T item0, T item1, params T[] itemsAfter)
		{
			return Many.Of(item0, item1, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 2
		//

		public static List<T> Of<T>(T item0, T item1, T item2)
		{
			return new List<T> { item0, item1, item2 };
		}

		public static List<T> Of<T>(T item0, T item1, T item2, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(item0, item1, item2);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(T item0, T item1, T item2, params T[] itemsAfter)
		{
			return Many.Of(item0, item1, item2, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 3
		//

		public static List<T> Of<T>(T item0, T item1, T item2, T item3)
		{
			return new List<T> { item0, item1, item2, item3 };
		}

		public static List<T> Of<T>(T item0, T item1, T item2, T item3, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(item0, item1, item2, item3);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(T item0, T item1, T item2, T item3, params T[] itemsAfter)
		{
			return Many.Of(item0, item1, item2, item3, itemsAfter as IEnumerable<T>);
		}

		//
		// Items after
		//

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, IEnumerable<T> itemsAfter)
		{
			var items = itemsBefore.ToList();

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 0 after
		//

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0)
		{
			var items = itemsBefore.ToList();

			items.Add(item0);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(itemsBefore, item0);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 1 after
		//

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1)
		{
			var items = Many.Of(itemsBefore, item0);

			items.Add(item1);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(itemsBefore, item0, item1);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 2 after
		//

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2)
		{
			var items = Many.Of(itemsBefore, item0, item1);

			items.Add(item2);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(itemsBefore, item0, item1, item2);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, itemsAfter as IEnumerable<T>);
		}

		//
		// Item 3 after
		//

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3)
		{
			var items = Many.Of(itemsBefore, item0, item1, item2);

			items.Add(item3);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, IEnumerable<T> itemsAfter)
		{
			var items = Many.Of(itemsBefore, item0, item1, item2, item3);

			items.AddRange(itemsAfter);

			return items;
		}

		public static List<T> Of<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, params T[] itemsAfter)
		{
			return Many.Of(itemsBefore, item0, item1, item2, item3, itemsAfter as IEnumerable<T>);
		}
	}
}