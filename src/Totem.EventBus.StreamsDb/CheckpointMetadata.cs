using Totem.Runtime;
using Totem.Timeline;

namespace Totem.EventBus.StreamsDb
{
  /// <summary>
  /// Data describing an event representing a flow checkpoint
  /// </summary>
  [Durable]
  public class CheckpointMetadata
  {
    public TimelinePosition Position;
    public TimelinePosition ErrorPosition;
    public string ErrorMessage;
    public bool IsDone;
  }
}