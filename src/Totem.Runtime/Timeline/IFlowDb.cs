using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes the database peristing instances of flow types
	/// </summary>
	public interface IFlowDb : IFluent
	{
		Flow ReadInstance(FlowType type);

		ClaimsPrincipal ReadPrincipal(TimelinePoint point);

		void AppendCall(WhenCall call);

		void AppendError(FlowType type, TimelinePoint point, Exception error);
	}
}