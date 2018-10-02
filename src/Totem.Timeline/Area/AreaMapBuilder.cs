using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Reflection;
using Totem.Runtime;
using Totem.Runtime.Metrics;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// Builds the map of a timeline area assembly
  /// </summary>
  internal sealed class AreaMapBuilder : Notion
  {
    readonly AreaMap _map;
    readonly Assembly _assembly;
    Type _type;

    internal AreaMapBuilder(AreaMap map, Assembly assembly)
    {
      _map = map;
      _assembly = assembly;
    }

    internal void Build()
    {
      var typesByIsEvent = _assembly.GetTypes().ToLookup(typeof(Event).IsAssignableFrom);

      foreach(var type in typesByIsEvent[true])
      {
        DeclareEvent(type);
      }

      foreach(var type in typesByIsEvent[false])
      {
        _type = type;

        _map.Durable.TryDeclare(type);

        if(!TryDeclareMetrics())
        {
          TryDeclareFlow();
        }
      }

      DeclareFlowStopped();
    }

    MapTypeInfo GetTypeInfo(Type type) =>
      new MapTypeInfo(_map.Key, type);

    MapTypeInfo GetTypeInfo() =>
      GetTypeInfo(_type);

    void DeclareEvent(Type type) =>
      _map.Events.Declare(new EventType(GetTypeInfo(type)));

    //
    // Metrics
    //

    bool TryDeclareMetrics()
    {
      if(!_type.IsStatic())
      {
        return false;
      }

      var metrics = Many.Of(
        from field in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        where typeof(Metric).IsAssignableFrom(field.FieldType)
        select new
        {
          Field = field,
          Key = MapTypeKey.From(_map.Key, $"{_type.Name}.{field.Name}"),
          Value = (Metric) field.GetValue(null)
        });

      if(!metrics.Any())
      {
        return false;
      }

      var type = new MetricsType(GetTypeInfo());

      _map.Metrics.Declare(type);

      foreach(var field in
        from metric in metrics
        select new MetricField(metric.Key, type, metric.Field, metric.Value))
      {
        type.Metrics.Declare(field);
      }

      return true;
    }

    IEnumerable<MetricField> ReadMetricFields(MetricsType type) =>
      from field in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
      where typeof(Metric).IsAssignableFrom(field.FieldType)
      let key = MapTypeKey.From(_map.Key, field.Name)
      let metric = (Metric) field.GetValue(null)
      select new MetricField(key, type, field, metric);

    //
    // Flows
    //

    void TryDeclareFlow()
    {
      if(typeof(Flow).IsAssignableFrom(_type))
      {
        var constructor = TryReadConstructor();

        if(constructor != null)
        {
          DeclareFlow(constructor, GetTypeInfo());
        }
      }
    }

    FlowConstructor TryReadConstructor()
    {
      var constructors = _type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

      switch(constructors.Length)
      {
        case 0:
          Log.Warning($"[runtime] Flow {_type} has no parameterless constructor; ignoring");

          return null;
        case 1:
          if(constructors[0].GetParameters().Length == 0)
          {
            return new FlowConstructor(constructors[0]);
          }

          Log.Warning($"[runtime] Flow {_type} constructor has parameters: {constructors[0]}; ignoring");

          return null;
        default:
          Log.Warning($"[runtime] Flow {_type} has multiple constructors; ignoring");

          return null;
      }
    }

    void DeclareFlow(FlowConstructor constructor, MapTypeInfo info)
    {
      if(typeof(Topic).IsAssignableFrom(_type))
      {
        var topic = new TopicType(info, constructor, ReadResumeAlgorithm());

        _map.Topics.Declare(topic);

        DeclareFlow(topic);
      }
      else if(typeof(Query).IsAssignableFrom(_type))
      {
        var query = new QueryType(info, constructor, ReadResumeAlgorithm(), ReadViewBatchSize());

        _map.Queries.Declare(query);

        DeclareFlow(query);
      }
      else
      {
        throw new NotSupportedException("Custom flow types require some thought");
      }
    }

    ResumeAlgorithm ReadResumeAlgorithm() =>
      _type.GetCustomAttribute<ResumeAlgorithmAttribute>()?.Algorithm ?? new ResumeAlgorithm();

    int ReadViewBatchSize() =>
      _type.GetCustomAttribute<QueryBatchSizeAttribute>(inherit: true).BatchSize;

    void DeclareFlow(FlowType flow)
    {
      _map.Flows.Declare(flow);

      new AreaMapFlowBuilder(_map, flow).Build();
    }

    void DeclareFlowStopped() =>
      _map.Events.Declare(new EventType(new MapTypeInfo(
        AreaKey.From("timeline"),
        typeof(FlowStopped))));
  }
}