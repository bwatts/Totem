namespace Totem.Timeline
{
  /// <summary>
  /// An error signal on the timeline of a distributed environment
  /// </summary>
  public abstract class ErrorEvent : Event
  {
    protected ErrorEvent(string error)
    {
      Error = error;
    }

    public readonly string Error;
  }
}