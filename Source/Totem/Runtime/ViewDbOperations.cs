using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Extends <see cref="IViewDb"/> with core operations
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ViewDbOperations
	{
		public static void DeleteByType(this IViewDb viewDb, Type viewType)
		{
			viewDb.Delete(viewType, ViewKey.Type);
		}

		public static void DeleteByType<TView>(this IViewDb viewDb) where TView : View
		{
			viewDb.Delete<TView>(ViewKey.Type);
		}

		public static View ReadByType(this IViewDb viewDb, Type viewType, bool strict = true)
		{
			return viewDb.Read(viewType, ViewKey.Type, strict);
		}

		public static TView ReadByType<TView>(this IViewDb viewDb, bool strict = true) where TView : View
		{
			return viewDb.Read<TView>(ViewKey.Type, strict);
		}
	}
}