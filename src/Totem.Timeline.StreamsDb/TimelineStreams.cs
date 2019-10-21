using Totem.Timeline.Area;

namespace Totem.Timeline.StreamsDb
{
  /// <summary>
  /// The names of streams in the timeline event store
  /// </summary>
  internal static class TimelineStreams
  {
    private const string Timeline = "timeline";
    private const string Schedule = "schedule";
    private const string Resume = "resume";
    private const string Client = "client";

    internal static string GetTimelineStream(string areaName)
    {
      return $"{areaName}-{Timeline}";
    }

    internal static string GetScheduleStream(string areaName)
    {
      return $"{areaName}-{Schedule}";
    }
    internal static string GetResumeStream(string areaName)
    {
      return $"{areaName}-{Resume}";
    }

    internal static string GetClientStream(string areaName)
    {
      return $"{areaName}-{Client}";
    }

    internal static string GetStream(this FlowKey key, string prefix, string suffix)
    {
      var subject = key.Type.IsSingleInstance ? key.ToString() : $"{key.Type}|{key.Id}";
      return $"{prefix}-{subject}-{suffix}";
    }

    internal static string GetCheckpointStream(this FlowKey key, string areaName) =>
      key.GetStream(areaName, "checkpoint");

    internal static string GetCheckpointStream(this FlowType type, string areaName) =>
      $"{areaName}-{type}-checkpoint";

    internal static string GetRoutesStream(this FlowKey key, string areaName) =>
      key.GetStream(areaName, "routes");

    internal static string GetRoutesStream(this AreaTypeName type, string areaName) =>
     $"{areaName}-{type}-routes";

    internal static string GetRoutesStream(this AreaTypeName type, string areaName, Id id) =>
     $"{areaName}-{type}|{id}-routes";
  }
}