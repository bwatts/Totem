using Totem.Timeline.Area;

namespace Totem.Timeline.Hosting
{
  /// <summary>
  /// Declares a timeline area to reside in the assembly of the derived type
  /// </summary>
  public abstract class TimelineArea
  {
    protected TimelineArea(string key)
    {
      Key = AreaKey.From(key);
    }

    public AreaKey Key { get; }

    public override string ToString() =>
      Key.ToString();

    public AreaMap BuildMap() =>
      new AreaMap(Key, GetType().Assembly);
  }
}