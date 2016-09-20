using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a set of <see cref="View"/> instances with snapshots accessible by key
	/// </summary>
	public interface IViewDb : IFluent
	{
		ViewSnapshot<string> ReadJsonSnapshot(Type type, Id id, TimelinePosition checkpoint);

		ViewSnapshot<View> ReadSnapshot(Type type, Id id, TimelinePosition checkpoint);

		ViewSnapshot<T> ReadSnapshot<T>(Id id, TimelinePosition checkpoint) where T : View;
	}
}