using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Describes the scope of a request's activity on the timeline
  /// </summary>
  internal interface IRequestScope : IConnectable
  {
    FlowKey Key { get; }

    void Push(FlowPoint point);

    void PushError(Exception error);
  }
}