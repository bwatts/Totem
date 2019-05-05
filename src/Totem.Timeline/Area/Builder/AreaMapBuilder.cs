using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Timeline.Area.Builder
{
  /// <summary>
  /// Builds a map of the types in a timeline area
  /// </summary>
  internal sealed class AreaMapBuilder
  {
    readonly Dictionary<Type, EventType> _eventsByDeclaredType = new Dictionary<Type, EventType>();
    readonly List<TopicType> _topics = new List<TopicType>();
    readonly List<QueryType> _queries = new List<QueryType>();
    readonly IEnumerable<AreaTypeInfo> _types;

    internal AreaMapBuilder(IEnumerable<AreaTypeInfo> types)
    {
      _types = types;
    }

    internal AreaMap Build()
    {
      DeclareEventsAndFlows();

      return CreateMap();
    }

    internal bool TryGetEvent(Type declaredType, out EventType eventType) =>
      _eventsByDeclaredType.TryGetValue(declaredType, out eventType);

    void DeclareEventsAndFlows()
    {
      var typesByIsEvent = _types.ToLookup(type => type.IsEvent);

      foreach(var eventType in typesByIsEvent[true])
      {
        DeclareEvent(eventType);
      }

      foreach(var flowType in typesByIsEvent[false])
      {
        DeclareFlow(flowType);
      }
    }

    void DeclareEvent(AreaTypeInfo info) =>
      _eventsByDeclaredType.Add(info.DeclaredType, new EventType(info));

    void DeclareFlow(AreaTypeInfo type)
    {
      var builder = new FlowTypeBuilder(this, type);

      if(builder.TryBuildTopic(out var topic))
      {
        _topics.Add(topic);
      }
      else
      {
        if(builder.TryBuildQuery(out var query))
        {
          _queries.Add(query);
        }
      }
    }

    AreaMap CreateMap() => new AreaMap(
      new AreaTypeSet<EventType>(_eventsByDeclaredType.Values),
      new AreaTypeSet<TopicType>(_topics),
      new AreaTypeSet<QueryType>(_queries));
  }
}