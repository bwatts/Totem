using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Reflection;
using Totem.Timeline.Area.Builder;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A map of the types in a timeline area
  /// </summary>
  public class AreaMap
  {
    internal AreaMap(AreaTypeSet<EventType> events, AreaTypeSet<TopicType> topics, AreaTypeSet<QueryType> queries)
    {
      Events = events;
      Topics = topics;
      Queries = queries;
    }

    public readonly AreaTypeSet<EventType> Events;
    public readonly AreaTypeSet<TopicType> Topics;
    public readonly AreaTypeSet<QueryType> Queries;

    public IEnumerable<FlowType> FlowTypes => Topics.Cast<FlowType>().Concat(Queries);
    public IEnumerable<AreaType> Types => Events.Cast<AreaType>().Concat(FlowTypes);

    public bool TryGet(TypeName name, out AreaType type)
    {
      if(Events.TryGet(name, out var e))
      {
        type = e;
      }
      else if(Topics.TryGet(name, out var topic))
      {
        type = topic;
      }
      else if(Queries.TryGet(name, out var query))
      {
        type = query;
      }
      else
      {
        type = null;
      }

      return type != null;
    }

    public bool TryGetFlow(TypeName name, out FlowType type)
    {
      if(Topics.TryGet(name, out var topic))
      {
        type = topic;
      }
      else if(Queries.TryGet(name, out var query))
      {
        type = query;
      }
      else
      {
        type = null;
      }

      return type != null;
    }

    public AreaType Get(TypeName name)
    {
      if(!TryGet(name, out var type))
      {
        throw new KeyNotFoundException($"This map does not contain a type with the specified name: {name}");
      }

      return type;
    }

    public FlowType GetFlow(TypeName name)
    {
      if(!TryGetFlow(name, out var type))
      {
        throw new KeyNotFoundException($"This map does not contain a flow with the specified name: {name}");
      }

      return type;
    }

    public static AreaMap From(IEnumerable<Type> potentialTypes)
    {
      var areaTypes = new List<AreaTypeInfo>();

      foreach(var type in potentialTypes)
      {
        if(AreaTypeInfo.TryFrom(type, out var areaType))
        {
          areaTypes.Add(areaType);
        }
      }

      return new AreaMapBuilder(areaTypes).Build();
    }
  }
}