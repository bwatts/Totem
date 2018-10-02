using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// Extends <see cref="AreaKey"/> and <see cref="FlowKey"/> to build stream names
  /// </summary>
  internal static class TimelineStreamExtensions
  {
    internal static string GetStream(this AreaMap area, string suffix) =>
      $"{area}-{suffix}";

    internal static string GetResumeStream(this AreaMap area) =>
      area.GetStream("resume");

    internal static string GetScheduleStream(this AreaMap area) =>
      area.GetStream("schedule");

    internal static string GetChangedQueriesStream(this AreaMap area) =>
      area.GetStream("changed-queries");

    internal static string GetStream(this FlowKey key, string suffix)
    {
      var prefix = key.Type.IsSingleInstance ? key.ToString() : $"{key.Type}|{key.Id}";

      return $"{prefix}-{suffix}";
    }

    internal static string GetCheckpointStream(this FlowKey key) =>
      key.GetStream("checkpoint");

    internal static string GetRoutesStream(this FlowKey key) =>
      key.GetStream("routes");
  }
}