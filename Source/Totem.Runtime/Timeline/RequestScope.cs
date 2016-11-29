using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The scope of a request's activity on the timeline
  /// </summary>
  internal sealed class RequestScope : FlowScope
  {
    internal RequestScope(ILifetimeScope lifetime, TimelineScope timeline, FlowRoute initialRoute)
      : base(lifetime, timeline, initialRoute)
    {}

    public void PushError(Exception error)
    {
      CompleteTask(error);
    }
  }
}