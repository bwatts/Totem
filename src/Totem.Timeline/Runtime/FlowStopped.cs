using Totem.Runtime;

namespace Totem.Timeline
{
  /// <summary>
  /// Indicates a flow encountered an unhandled error and is no longer observing the timeline
  /// </summary>
  public sealed class FlowStopped : Event
  {
    public FlowStopped(FlowKey key, string error)
    {
      Key = key;
      Error = error;
    }

    public readonly FlowKey Key;
    public readonly string Error;
  }
}