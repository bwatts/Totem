using System;
using Totem.Metrics;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Monitors the performance of aspects of the timeline
  /// </summary>
  internal static class TimelineMetrics
  {
    internal static readonly Duration PushTime = new Duration();
    internal static readonly Activity FlowActivity = new Activity();
    internal static readonly Duration WhenTime = new Duration();
    internal static readonly Duration GivenTime = new Duration();

    internal static MetricPath ToPath(this Flow flow) =>
      flow.Context.Key.ToString();
  }
}