using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Describes a database persisting data pertaining to flows
  /// </summary>
  public interface IFlowDb
  {
    ClaimsPrincipal ReadPrincipal(TimelinePoint point);

    Flow ReadInstance(FlowKey key);

    void WriteCall(WhenCall call);

    void WriteError(FlowKey key, TimelinePoint point, Exception error);
  }
}