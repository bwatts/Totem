using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a set of <see cref="View"/> instances accessible by key
	/// </summary>
	public interface IViewDb : IFluent
	{
    View Read(Type viewType, Id id, bool strict = true);

    string ReadJson(Type viewType, Id id, bool strict = true);
	}
}