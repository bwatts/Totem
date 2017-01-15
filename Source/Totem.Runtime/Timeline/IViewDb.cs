using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a set of <see cref="View"/> instances with snapshots accessible by key
	/// </summary>
	public interface IViewDb : IFluent
	{
		Task<ViewSnapshot<string>> ReadJsonSnapshot(Type type, Id id, TimelinePosition checkpoint);

    Task<ViewSnapshot<View>> ReadSnapshot(Type type, Id id, TimelinePosition checkpoint);

    Task<ViewSnapshot<T>> ReadSnapshot<T>(Id id, TimelinePosition checkpoint) where T : View;
	}
}