namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// Indicates a command failed when applied to an area
  /// </summary>
  public sealed class CommandFailed : Event
  {
    public CommandFailed(Id commandId, string error)
    {
      CommandId = commandId;
      Error = error;
    }

    public readonly Id CommandId;
    public readonly string Error;
  }
}