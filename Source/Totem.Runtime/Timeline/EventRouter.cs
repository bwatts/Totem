using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Routes timeline points to flows that observe an event
  /// </summary>
  internal sealed class EventRouter
  {
    private readonly List<FlowRouter> _flows = new List<FlowRouter>();
    private readonly EventType _event;

    internal EventRouter(EventType e)
    {
      _event = e;
    }

    public override string ToString()
    {
      return $"{_event} ({Text.Count(_flows.Count, "flow")})";
    }

    internal void RegisterFlow(FlowRouter flow)
    {
      _flows.Add(flow);
    }

    internal void Push(TimelinePoint point)
    {
      foreach(var flow in _flows)
      {
        flow.Push(point);
      }
    }
  }
}