using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes a scope in which timeline points occur synchronously
	/// </summary>
	public interface ITimelineScope : IConnectable
	{
		void Push(TimelinePoint point);
	}
}