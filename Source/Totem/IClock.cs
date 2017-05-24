using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Describes a context in which events occur on the same timeline
	/// </summary>
	public interface IClock : IClean
	{
		DateTime Now { get; }
	}
}