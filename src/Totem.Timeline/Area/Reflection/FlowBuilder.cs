using System;
using System.Reflection;
using Totem.Runtime;

namespace Totem.Timeline.Area.Reflection
{
  /// <summary>
  /// Builds a flow in a timeline area map
  /// </summary>
  internal sealed class FlowBuilder : Notion
  {
    readonly MapBuilder _map;
    readonly AreaKey _area;
    readonly Type _declaredType;

    internal FlowBuilder(MapBuilder map, AreaKey area, Type declaredType)
    {
      _map = map;
      _area = area;
      _declaredType = declaredType;
    }

    internal bool TryBuildTopic(out TopicType topic) =>
      TryBuildFlow(typeof(Topic), info => new TopicType(info), out topic);

    internal bool TryBuildQuery(out QueryType query) =>
      TryBuildFlow(typeof(Query), info => new QueryType(info, GetQueryBatchSize()), out query);

    bool TryBuildFlow<T>(Type baseType, Func<FlowTypeInfo, T> createFlow, out T flow) where T : FlowType
    {
      flow = null;

      if(baseType.IsAssignableFrom(_declaredType))
      {
        if(TryGetInfo(out var info))
        {
          flow = createFlow(info);

          DeclareObservations(flow);
        }
      }

      return flow != null;
    }

    bool TryGetInfo(out FlowTypeInfo info)
    {
      info = null;

      if(TryGetConstructor(out var constructor))
      {
        info = new FlowTypeInfo(
          _area,
          _declaredType,
          constructor,
          new FlowObservationSet(),
          GetResumeAlgorithm());
      }

      return info != null;
    }

    bool TryGetConstructor(out FlowConstructor constructor)
    {
      constructor = null;

      var constructors = _declaredType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

      if(constructors.Length == 0)
      {
        Log.Warning("[timeline] Flow {Type} has no public parameterless constructor; ignoring", _declaredType);
      }
      else
      {
        if(constructors.Length == 1)
        {
          if(constructors[0].GetParameters().Length == 0)
          {
            constructor = new FlowConstructor(constructors[0]);
          }
          else
          {
            Log.Warning("[timeline] Flow {Type} constructor has parameters; ignoring", _declaredType);
          }
        }
      }

      return constructor != null;
    }

    ResumeAlgorithm GetResumeAlgorithm() =>
      _declaredType.GetCustomAttribute<ResumeAlgorithmAttribute>()?.Algorithm ?? new ResumeAlgorithm();

    int GetQueryBatchSize() =>
      _declaredType.GetCustomAttribute<QueryBatchSizeAttribute>(inherit: true).BatchSize;

    void DeclareObservations(FlowType flow) =>
      new FlowObservationBuilder(_map, flow).Build();
  }
}