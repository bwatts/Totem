using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Extends <see cref="IViewDb"/> with core operations
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ViewDbOperations
  {
    public static View Read(this IViewDb viewDb, Type viewType, Id id, bool strict = true)
    {
      return viewDb.Read(viewType, id, strict);
    }

    public static TView Read<TView>(this IViewDb viewDb, Id id, bool strict = true) where TView : View
    {
      return (TView) viewDb.Read(typeof(TView), id, strict);
    }

    public static View Read(this IViewDb viewDb, ViewType viewType, Id id, bool strict = true)
    {
      return viewDb.Read(viewType, Id.Unassigned, strict);
    }

    public static View Read(this IViewDb viewDb, Type viewType, bool strict = true)
    {
      return viewDb.Read(viewType, Id.Unassigned, strict);
    }

    public static TView Read<TView>(this IViewDb viewDb, bool strict = true) where TView : View
    {
      return viewDb.Read<TView>(Id.Unassigned, strict);
    }

    public static View Read(this IViewDb viewDb, ViewType viewType, bool strict = true)
    {
      return viewDb.Read(viewType, Id.Unassigned, strict);
    }

    //
    // JSON
    //

    public static string ReadJson(this IViewDb viewDb, Type viewType, Id id, bool strict = true)
    {
      return viewDb.ReadJson(viewType, id, strict);
    }

    public static string ReadJson<TView>(this IViewDb viewDb, Id id, bool strict = true) where TView : View
    {
      return viewDb.ReadJson(typeof(TView), id, strict);
    }

    public static string ReadJson(this IViewDb viewDb, ViewType viewType, Id id, bool strict = true)
    {
      return viewDb.ReadJson(viewType, id, strict);
    }

    public static string ReadJson(this IViewDb viewDb, Type viewType, bool strict = true)
    {
      return viewDb.ReadJson(viewType, Id.Unassigned, strict);
    }

    public static string ReadJson<TView>(this IViewDb viewDb, bool strict = true) where TView : View
    {
      return viewDb.ReadJson<TView>(Id.Unassigned, strict);
    }

    public static string ReadJson(this IViewDb viewDb, ViewType viewType, bool strict = true)
    {
      return viewDb.ReadJson(viewType, Id.Unassigned, strict);
    }
  }
}