using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Totem.Timeline.Area.Reflection
{
  /// <summary>
  /// Builds the map of a timeline area assembly
  /// </summary>
  internal sealed class MapBuilder
  {
    readonly Dictionary<Type, EventType> _eventsByDeclaredType = new Dictionary<Type, EventType>();
    readonly List<TopicType> _topics = new List<TopicType>();
    readonly List<QueryType> _queries = new List<QueryType>();
    readonly AreaKey _key;
    readonly Assembly _assembly;

    internal MapBuilder(AreaKey key, Assembly assembly)
    {
      _key = key;
      _assembly = assembly;
    }

    internal AreaMap Build()
    {
      DeclareEventsAndFlows();

      DeclareFlowStopped();

      return CreateMap();
    }

    internal bool TryGetEvent(Type declaredType, out EventType eventType) =>
      _eventsByDeclaredType.TryGetValue(declaredType, out eventType);

    void DeclareEventsAndFlows()
    {
      var typesByIsEvent = _assembly.GetTypes().ToLookup(typeof(Event).IsAssignableFrom);

      foreach(var eventType in typesByIsEvent[true])
      {
        DeclareEvent(new MapTypeInfo(_key, eventType));
      }

      foreach(var type in typesByIsEvent[false])
      {
        TryDeclareFlow(type);
      }
    }

    void DeclareEvent(MapTypeInfo info) =>
      _eventsByDeclaredType.Add(info.DeclaredType, new EventType(info));

    void TryDeclareFlow(Type type)
    {
      var builder = new FlowBuilder(this, _key, type);

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

    void DeclareFlowStopped() =>
      DeclareEvent(new MapTypeInfo(AreaKey.From("timeline"), typeof(FlowStopped)));

    AreaMap CreateMap() =>
      new AreaMap(
        _key,
        new MapTypeSet<EventType>(_eventsByDeclaredType.Values),
        new MapTypeSet<TopicType>(_topics),
        new MapTypeSet<QueryType>(_queries));
  }
}