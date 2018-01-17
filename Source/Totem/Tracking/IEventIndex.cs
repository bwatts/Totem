namespace Totem.Tracking
{
  /// <summary>
  /// Describes an index of tracked timeline events
  /// </summary>
  public interface IEventIndex
	{
		void Index(TrackedEvent e);
	}
}