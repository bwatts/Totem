using Totem.Timeline.Area;
using Totem.Timeline.Area.Reflection;

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

    public readonly AreaKey Key;

    public override string ToString() =>
      Key.ToString();

    public AreaMap BuildMap() =>
      new MapBuilder(Key, GetType().Assembly).Build();
  }
}