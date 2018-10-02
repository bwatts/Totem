using System.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A map of the elements in a timeline area
  /// </summary>
  public class AreaMap
  {
    public AreaMap(AreaKey key, Assembly assembly)
    {
      Key = key;
      Durable = new DurableTypeSet(key);

      new AreaMapBuilder(this, assembly).Build();
    }

    public readonly AreaKey Key;
    public readonly DurableTypeSet Durable;
    public readonly MapTypeSet<FlowType> Flows = new MapTypeSet<FlowType>();
    public readonly MapTypeSet<TopicType> Topics = new MapTypeSet<TopicType>();
    public readonly MapTypeSet<QueryType> Queries = new MapTypeSet<QueryType>();
    public readonly MapTypeSet<EventType> Events = new MapTypeSet<EventType>();
    public readonly MapTypeSet<MetricsType> Metrics = new MapTypeSet<MetricsType>();

    public override string ToString() =>
      Key.ToString();
  }
}