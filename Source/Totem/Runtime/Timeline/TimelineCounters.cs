using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Diagnostics;

namespace Totem.Runtime.Timeline
{
  [SingleInstanceCategory("Timeline", "Monitors timeline performance")]
  internal static class TimelineCounters
  {
    internal static readonly ElapsedTime Uptime =
      Counter.ElapsedTime("Uptime", "Time available");

    internal static readonly Count32 EventsInProgress =
      Counter.Count32("Events handling", "Count of events being handled");

    internal static readonly Total32 EventsHandled =
      Counter.Total32("Events handled", "Total events handled by flows");

    internal static readonly AverageTime EventTime =
      Counter.AverageTime("Event time", "Average time to handle an event");
  }
}