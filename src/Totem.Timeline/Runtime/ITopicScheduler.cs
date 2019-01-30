using System;

namespace Totem.Timeline
{
  /// <summary>
  /// Describes the scheduling of events for a topic
  /// </summary>
  public interface ITopicScheduler
  {
    DateTimeOffset Now { get; }

    void At(Event e, DateTimeOffset whenOccurs);
  }
}