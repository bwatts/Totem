namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// The names of streams in the timeline event store
  /// </summary>
  internal static class TimelineStreams
  {
    internal const string Timeline = "timeline";
    internal const string Schedule = "schedule";
    internal const string Resume = "resume";
    internal const string Client = "client";

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