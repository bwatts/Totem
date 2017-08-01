using Totem.Metrics;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// Monitors the performance of aspects of pushes to the timeline
  /// </summary>
  internal static class TimelinePushMetrics
  {
    internal static readonly Count Open = new Count();
    internal static readonly Count Done = new Count();
    internal static readonly Count Group = new Count();
    internal static readonly Duration Time = new Duration();
  }
}