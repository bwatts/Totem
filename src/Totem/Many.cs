using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// Composes lists from sequences of various shapes
  /// </summary>
  public static class Many
  {
    public static Many<T> Of<T>() =>
      new Many<T>(new List<T>());

    public static Many<T> Of<T>(IEnumerable<T> items) =>
      new Many<T>(items as List<T> ?? items.ToList());

    public static Many<T> Of<T>(params T[] items) =>
      new Many<T>(items.ToList());

    //
    // Item 0
    //

    public static Many<T> Of<T>(T item0)
    {
      var items = Of<T>();

      items.Write.Add(item0);

      return items;
    }

    public static Many<T> Of<T>(T item0, IEnumerable<T> itemsAfter)
    {
      var items = Of(item0);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> Of<T>(T item0, params T[] itemsAfter) =>
      Of(item0, itemsAfter as IEnumerable<T>);

    //
    // Item 1
    //

    public static Many<T> Of<T>(T item0, T item1)
    {
      var items = Of(item0);

      items.Write.Add(item1);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, IEnumerable<T> itemsAfter)
    {
      var items = Of(item0, item1);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, params T[] itemsAfter)
    {
      return Of(item0, item1, itemsAfter as IEnumerable<T>);
    }

    //
    // Item 2
    //

    public static Many<T> Of<T>(T item0, T item1, T item2)
    {
      var items = Of(item0, item1);

      items.Write.Add(item2);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, T item2, IEnumerable<T> itemsAfter)
    {
      var items = Of(item0, item1, item2);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, T item2, params T[] itemsAfter)
    {
      return Of(item0, item1, item2, itemsAfter as IEnumerable<T>);
    }

    //
    // Item 3
    //

    public static Many<T> Of<T>(T item0, T item1, T item2, T item3)
    {
      var items = Of(item0, item1, item2);

      items.Write.Add(item3);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, T item2, T item3, IEnumerable<T> itemsAfter)
    {
      var items = Of(item0, item1, item2, item3);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> Of<T>(T item0, T item1, T item2, T item3, params T[] itemsAfter) =>
      Of(item0, item1, item2, item3, itemsAfter as IEnumerable<T>);

    //
    // All
    //

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, IEnumerable<T> itemsAfter)
    {
      var items = Of(itemsBefore);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, params T[] itemsAfter) =>
      OfAll(itemsBefore, itemsAfter as IEnumerable<T>);

    //
    // All - Item 0
    //

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0)
    {
      var items = Of(itemsBefore);

      items.Write.Add(item0);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, IEnumerable<T> itemsAfter)
    {
      var items = OfAll(itemsBefore, item0);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, params T[] itemsAfter) =>
      OfAll(itemsBefore, item0, itemsAfter as IEnumerable<T>);

    //
    // All - Item 1
    //

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1)
    {
      var items = OfAll(itemsBefore, item0);

      items.Write.Add(item1);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, IEnumerable<T> itemsAfter)
    {
      var items = OfAll(itemsBefore, item0, item1);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, params T[] itemsAfter) =>
      OfAll(itemsBefore, item0, item1, itemsAfter as IEnumerable<T>);

    //
    // All - Item 2
    //

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2)
    {
      var items = OfAll(itemsBefore, item0, item1);

      items.Write.Add(item2);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, IEnumerable<T> itemsAfter)
    {
      var items = OfAll(itemsBefore, item0, item1, item2);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, params T[] itemsAfter) =>
      OfAll(itemsBefore, item0, item1, item2, itemsAfter as IEnumerable<T>);

    //
    // All - Item 3
    //

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3)
    {
      var items = OfAll(itemsBefore, item0, item1, item2);

      items.Write.Add(item3);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, IEnumerable<T> itemsAfter)
    {
      var items = OfAll(itemsBefore, item0, item1, item2, item3);

      items.Write.AddRange(itemsAfter);

      return items;
    }

    public static Many<T> OfAll<T>(IEnumerable<T> itemsBefore, T item0, T item1, T item2, T item3, params T[] itemsAfter) =>
      OfAll(itemsBefore, item0, item1, item2, item3, itemsAfter as IEnumerable<T>);
  }

  /// <summary>
  /// A read-only list of items of the specified type with explicit access to a writable interface
  /// </summary>
  /// <remarks>
  /// This should not implement IList, even explicitly. Extension methods still appear, breaking the immutable illusion.
  /// However, JSON.NET doesn't understand the type without IList; telling it otherwise is somewhat involved.
  /// Determine how to remove that interface.
  /// </remarks>
  /// <typeparam name="T">The type of items in the list</typeparam>
  [Serializable]
  [DebuggerDisplay("Count = {Count}")]
  public class Many<T> : IReadOnlyList<T>, IList<T>
  {
    readonly List<T> _items;

    public Many()
    {
      _items = new List<T>();
    }

    internal Many(List<T> items)
    {
      _items = items;
    }

    public IList<T> Write => _items;

    public IEnumerator<T> GetEnumerator() =>
      _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
      GetEnumerator();

    public T this[int index] => _items[index];

    public int Count => _items.Count;
    public bool IsEmpty => Count == 0;
    public bool IsNotEmpty => Count > 0;

    public int IndexOf(T item) =>
      _items.IndexOf(item);

    public bool Contains(T item) =>
      _items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) =>
      _items.CopyTo(array, arrayIndex);

    //
    // IList
    //

    bool ICollection<T>.IsReadOnly =>
      ((ICollection<T>) _items).IsReadOnly;

    void IList<T>.Insert(int index, T item) =>
      _items.Insert(index, item);

    void IList<T>.RemoveAt(int index) =>
      _items.RemoveAt(index);

    T IList<T>.this[int index]
    {
      get { return _items[index]; }
      set { _items[index] = value; }
    }

    void ICollection<T>.Add(T item) =>
      _items.Add(item);

    void ICollection<T>.Clear() =>
      _items.Clear();

    bool ICollection<T>.Remove(T item) =>
      _items.Remove(item);
  }
}