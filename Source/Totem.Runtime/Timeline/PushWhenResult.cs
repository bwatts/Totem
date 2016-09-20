using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The result of pushing a <see cref="FlowCall.When"/> to the timeline
  /// </summary>
  public class PushWhenResult
  {
    public PushWhenResult(Many<TimelineMessage> messages, bool flowStopped)
    {
      Messages = messages;
      FlowStopped = flowStopped;
    }

    public PushWhenResult() : this(new Many<TimelineMessage>(), false)
    {}

    public readonly Many<TimelineMessage> Messages;
    public readonly bool FlowStopped;
  }
}