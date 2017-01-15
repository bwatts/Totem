using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

		public static Task<ViewSnapshot<View>> ReadSnapshot(this IViewDb viewDb, FlowKey key, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(key.Type.DeclaredType, key.Id, checkpoint);
		}

		public static Task<ViewSnapshot<View>> ReadSnapshot(this IViewDb viewDb, ViewType type, Id id, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type.DeclaredType, id, checkpoint);
		}

		public static Task<ViewSnapshot<View>> ReadSnapshot(this IViewDb viewDb, ViewType type, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static Task<ViewSnapshot<View>> ReadSnapshot(this IViewDb viewDb, Type type, TimelinePosition checkpoint)
		{
			return viewDb.ReadSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static Task<ViewSnapshot<T>> ReadSnapshot<T>(this IViewDb viewDb, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadSnapshot<T>(Id.Unassigned, checkpoint);
		}

		//
		// Snapshots (JSON)
		//

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot(this IViewDb viewDb, FlowKey key, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(key.Type.DeclaredType, key.Id, checkpoint);
		}

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot(this IViewDb viewDb, ViewType type, Id id, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type.DeclaredType, id, checkpoint);
		}

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot(this IViewDb viewDb, ViewType type, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot(this IViewDb viewDb, Type type, TimelinePosition checkpoint)
		{
			return viewDb.ReadJsonSnapshot(type, Id.Unassigned, checkpoint);
		}

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot<T>(this IViewDb viewDb, Id id, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadJsonSnapshot(typeof(T), id, checkpoint);
		}

		public static Task<ViewSnapshot<string>> ReadJsonSnapshot<T>(this IViewDb viewDb, TimelinePosition checkpoint) where T : View
		{
			return viewDb.ReadJsonSnapshot<T>(Id.Unassigned, checkpoint);
		}

		//
		// Content
		//

		public static Task<View> Read(this IViewDb viewDb, FlowKey key, bool strict = true)
		{
      return ReadContent(viewDb.ReadSnapshot(key, TimelinePosition.None), strict);
		}

		public static Task<View> Read(this IViewDb viewDb, ViewType type, Id id, bool strict = true)
		{
			return ReadContent(viewDb.ReadSnapshot(type, id, TimelinePosition.None), strict);
		}

		public static Task<View> Read(this IViewDb viewDb, ViewType type, bool strict = true)
		{
			return ReadContent(viewDb.ReadSnapshot(type, TimelinePosition.None), strict);
		}

		public static Task<View> Read(this IViewDb viewDb, Type type, Id id, bool strict = true)
		{
			return ReadContent(viewDb.ReadSnapshot(type, id, TimelinePosition.None), strict);
		}

		public static Task<View> Read(this IViewDb viewDb, Type type, bool strict = true)
		{
			return ReadContent(viewDb.ReadSnapshot(type, TimelinePosition.None), strict);
		}

		public static Task<T> Read<T>(this IViewDb viewDb, Id id, bool strict = true) where T : View
		{
			return ReadContent(viewDb.ReadSnapshot<T>(id, TimelinePosition.None), strict);
		}

		public static Task<T> Read<T>(this IViewDb viewDb, bool strict = true) where T : View
		{
			return ReadContent(viewDb.ReadSnapshot<T>(TimelinePosition.None), strict);
		}

		//
		// Content (JSON)
		//

		public static Task<string> ReadJson(this IViewDb viewDb, FlowKey key, bool strict = true)
		{
			return ReadContent(viewDb.ReadJsonSnapshot(key, TimelinePosition.None), strict);
		}

		public static Task<string> ReadJson(this IViewDb viewDb, ViewType type, Id id, bool strict = true)
		{
			return ReadContent(viewDb.ReadJsonSnapshot(type, id, TimelinePosition.None), strict);
		}

		public static Task<string> ReadJson(this IViewDb viewDb, ViewType type, bool strict = true)
		{
			return ReadContent(viewDb.ReadJsonSnapshot(type, TimelinePosition.None), strict);
		}

		public static Task<string> ReadJson(this IViewDb viewDb, Type type, bool strict = true)
		{
			return ReadContent(viewDb.ReadJsonSnapshot(type, TimelinePosition.None), strict);
		}

		public static Task<string> ReadJson<T>(this IViewDb viewDb, Id id, bool strict = true) where T : View
		{
			return ReadContent(viewDb.ReadJsonSnapshot<T>(id, TimelinePosition.None), strict);
		}

		public static Task<string> ReadJson<T>(this IViewDb viewDb, bool strict = true) where T : View
		{
			return ReadContent(viewDb.ReadJsonSnapshot<T>(TimelinePosition.None), strict);
		}

		static async Task<T> ReadContent<T>(this Task<ViewSnapshot<T>> readView, bool strict)
		{
      var view = await readView;

			Expect.False(strict && view.NotFound, "View not found");

			return view.NotFound ? default(T) : view.ReadContent();
		}
	}
}