using System;
using Totem.Metrics;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Monitors the performance of aspects of the timeline
  /// </summary>
  public static class TimelineMetrics
  {
    public static readonly Duration TimeToObserve = new Duration();

    internal static readonly Activity FlowActivity = new Activity();
    internal static readonly Duration WhenTime = new Duration();
    internal static readonly Duration GivenTime = new Duration();

    internal static MetricPath ToPath(this Flow flow) =>
      $"{flow.Context.Key}";

    internal static MetricPath ToPath(this Flow flow, TimelinePosition position) =>
      $"{flow.Context.Key}@{position}";

    internal static MetricPath ToPath(this TimelinePoint point) =>
      $"{point.EventType}@{point.Position}";

    internal static MetricPath ToPath(this TimelinePoint point, string prefix) =>
      $"{prefix}/{point.EventType}@{point.Position}";
  }
}