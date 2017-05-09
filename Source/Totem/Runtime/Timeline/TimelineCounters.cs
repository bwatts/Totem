using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Diagnostics;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Performance monitoring for aspects of the timeline
  /// </summary>
  [Counters("Timeline", "Monitors timeline performance")]
  internal static class TimelineCounters
  {
    internal static readonly ElapsedTime Uptime =
      Counter.ElapsedTime("Uptime", "Time available");

    internal static readonly Count EventsInProgress =
      Counter.Count("Events handling", "Count of events being handled");

    internal static readonly Total EventsHandled =
      Counter.Total("Events handled", "Total events handled by flows");

    internal static readonly AverageTime EventTime =
      Counter.AverageTime("Event time", "Average time to handle an event");
  }
}