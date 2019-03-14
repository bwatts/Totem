using System.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A method observing an event within a <see cref="Flow"/>
  /// </summary>
  public abstract class FlowMethod
  {
    internal FlowMethod(MethodInfo info, EventType eventType)
    {
      Info = info;
      EventType = eventType;
    }

    public readonly MethodInfo Info;
    public readonly EventType EventType;
  }
}