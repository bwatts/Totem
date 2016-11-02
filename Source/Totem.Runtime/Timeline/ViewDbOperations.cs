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
		//
		// Snapshots
		//

		public static ViewSnapshot<View> ReadSnapshot(this IViewDb viewDb, FlowKey key, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(key.Type.DeclaredType, key.Id, checkpoint);
		}

		public static ViewSnapshot<View> ReadSnapshot(this IViewDb viewDb, ViewType type, Id id, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type.DeclaredType, id, checkpoint);
		}

		public static ViewSnapshot<View> ReadSnapshot(this IViewDb viewDb, ViewType type, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static ViewSnapshot<View> ReadSnapshot(this IViewDb viewDb, Type type, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static ViewSnapshot<T> ReadSnapshot<T>(this IViewDb viewDb, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadSnapshot<T>(Id.Unassigned, checkpoint);
		}

		//
		// Snapshots (JSON)
		//

		public static ViewSnapshot<string> ReadJsonSnapshot(this IViewDb viewDb, FlowKey key, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(key.Type.DeclaredType, key.Id, checkpoint);
		}

		public static ViewSnapshot<string> ReadJsonSnapshot(this IViewDb viewDb, ViewType type, Id id, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type.DeclaredType, id, checkpoint);
		}

		public static ViewSnapshot<string> ReadJsonSnapshot(this IViewDb viewDb, ViewType type, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static ViewSnapshot<string> ReadJsonSnapshot(this IViewDb viewDb, Type type, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static ViewSnapshot<string> ReadJsonSnapshot<T>(this IViewDb viewDb, Id id, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadJsonSnapshot(typeof(T), id, checkpoint);
		}

		public static ViewSnapshot<string> ReadJsonSnapshot<T>(this IViewDb viewDb, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadJsonSnapshot<T>(Id.Unassigned, checkpoint);
		}

		//
		// Content
		//

		public static View Read(this IViewDb viewDb, FlowKey key, bool strict = true)
		{
			return viewDb.ReadSnapshot(key, TimelinePosition.None).ReadContent(strict);
		}

		public static View Read(this IViewDb viewDb, ViewType type, Id id, bool strict = true)
		{
			return viewDb.ReadSnapshot(type, id, TimelinePosition.None).ReadContent(strict);
		}

		public static View Read(this IViewDb viewDb, ViewType type, bool strict = true)
		{
			return viewDb.ReadSnapshot(type, TimelinePosition.None).ReadContent(strict);
		}

		public static View Read(this IViewDb viewDb, Type type, Id id, bool strict = true)
		{
			return viewDb.ReadSnapshot(type, id, TimelinePosition.None).ReadContent(strict);
		}

		public static View Read(this IViewDb viewDb, Type type, bool strict = true)
		{
			return viewDb.ReadSnapshot(type, TimelinePosition.None).ReadContent(strict);
		}

		public static T Read<T>(this IViewDb viewDb, Id id, bool strict = true) where T : View
		{
			return viewDb.ReadSnapshot<T>(id, TimelinePosition.None).ReadContent(strict);
		}

		public static T Read<T>(this IViewDb viewDb, bool strict = true) where T : View
		{
			return viewDb.ReadSnapshot<T>(TimelinePosition.None).ReadContent(strict);
		}

		//
		// Content (JSON)
		//

		public static string ReadJson(this IViewDb viewDb, FlowKey key, bool strict = true)
		{
			return viewDb.ReadJsonSnapshot(key, TimelinePosition.None).ReadContent(strict);
		}

		public static string ReadJson(this IViewDb viewDb, ViewType type, Id id, bool strict = true)
		{
			return viewDb.ReadJsonSnapshot(type, id, TimelinePosition.None).ReadContent(strict);
		}

		public static string ReadJson(this IViewDb viewDb, ViewType type, bool strict = true)
		{
			return viewDb.ReadJsonSnapshot(type, TimelinePosition.None).ReadContent(strict);
		}

		public static string ReadJson(this IViewDb viewDb, Type type, bool strict = true)
		{
			return viewDb.ReadJsonSnapshot(type, TimelinePosition.None).ReadContent(strict);
		}

		public static string ReadJson<T>(this IViewDb viewDb, Id id, bool strict = true) where T : View
		{
			return viewDb.ReadJsonSnapshot<T>(id, TimelinePosition.None).ReadContent(strict);
		}

		public static string ReadJson<T>(this IViewDb viewDb, bool strict = true) where T : View
		{
			return viewDb.ReadJsonSnapshot<T>(TimelinePosition.None).ReadContent(strict);
		}

		private static T ReadContent<T>(this ViewSnapshot<T> view, bool strict)
		{
			Expect.False(strict && view.NotFound, "View not found");

			return view.NotFound ? default(T) : view.ReadContent();
		}
	}
}