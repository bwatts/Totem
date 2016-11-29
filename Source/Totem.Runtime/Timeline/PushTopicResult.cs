using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// The result of pushing a topic to the timeline
  /// </summary>
  public class PushTopicResult
  {
    public PushTopicResult(Many<TimelineMessage> messages, bool givenError)
    {
      Messages = messages;
      GivenError = givenError;
    }

    public PushTopicResult() : this(new Many<TimelineMessage>(), false)
    {}

    public readonly Many<TimelineMessage> Messages;
    public readonly bool GivenError;
  }
}