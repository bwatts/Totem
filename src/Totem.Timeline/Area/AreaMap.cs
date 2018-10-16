using System.Collections.Generic;
using System.Linq;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A map of the elements in an area of the timeline
  /// </summary>
  public class AreaMap
  {
    public AreaMap(
      AreaKey key,
      MapTypeSet<EventType> events,
      MapTypeSet<TopicType> topics,
      MapTypeSet<QueryType> queries)
    {
      Key = key;
      Events = events;
      Topics = topics;
      Queries = queries;
    }

    public readonly AreaKey Key;
    public readonly MapTypeSet<EventType> Events;
    public readonly MapTypeSet<TopicType> Topics;
    public readonly MapTypeSet<QueryType> Queries;

    public override string ToString() =>
      Key.ToString();

    public IEnumerable<MapType> Types =>
      Events.Cast<MapType>().Concat(FlowTypes);

    public IEnumerable<FlowType> FlowTypes =>
      Topics.Cast<FlowType>().Concat(Queries);

    public bool TryGet(MapTypeKey key, out MapType type)
    {
      if(Events.TryGet(key, out var e))
      {
        type = e;
      }
      else if(Topics.TryGet(key, out var topic))
      {
        type = topic;
      }
      else if(Queries.TryGet(key, out var query))
      {
        type = query;
      }
      else
      {
        type = null;
      }

      return type != null;
    }

    public bool TryGetFlow(MapTypeKey key, out FlowType type)
    {
      if(Topics.TryGet(key, out var topic))
      {
        type = topic;
      }
      else if(Queries.TryGet(key, out var query))
      {
        type = query;
      }
      else
      {
        type = null;
      }

      return type != null;
    }

    public MapType Get(MapTypeKey key)
    {
      if(!TryGet(key, out var type))
      {
        throw new KeyNotFoundException($"The map does not contain the specified key: {key}");
      }

      return type;
    }

    public FlowType GetFlow(MapTypeKey key)
    {
      if(!TryGetFlow(key, out var type))
      {
        throw new KeyNotFoundException($"The map does not contain the specified flow key: {key}");
      }

      return type;
    }
  }
}