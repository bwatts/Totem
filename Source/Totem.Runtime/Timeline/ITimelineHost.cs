using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the hosting of timeline activity by a runtime
	/// </summary>
	public interface ITimelineHost : ITimelineScope
	{
		Task<TFlow> MakeRequest<TFlow>(Id id) where TFlow : Request;
	}
}