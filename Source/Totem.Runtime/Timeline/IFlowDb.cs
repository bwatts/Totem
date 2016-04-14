using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Describes a database persisting data pertaining to flows
  /// </summary>
  public interface IFlowDb
  {
    ClaimsPrincipal ReadPrincipal(TimelinePoint point);

		bool TryReadFlow(FlowType type, TimelineRoute route, out Flow flow);

    void WriteCall(WhenCall call);

    void WriteError(FlowKey key, TimelinePoint point, Exception error);
  }
}