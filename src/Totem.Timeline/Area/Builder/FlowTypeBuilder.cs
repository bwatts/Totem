using System.Reflection;
using Totem.Runtime;

namespace Totem.Timeline.Area.Builder
{
  /// <summary>
  /// Builds a flow type in a timeline area
  /// </summary>
  internal sealed class FlowTypeBuilder : Notion
  {
    readonly AreaMapBuilder _map;
    readonly AreaTypeInfo _info;

    internal FlowTypeBuilder(AreaMapBuilder map, AreaTypeInfo info)
    {
      _map = map;
      _info = info;
    }

    internal bool TryBuildTopic(out TopicType topic)
    {
      topic = null;

      if(_info.IsTopic)
      {
        if(TryGetConstructor(out var constructor))
        {
          topic = new TopicType(_info, constructor, new FlowObservationSet(), GetResumeAlgorithm());

          DeclareObservations(topic);
        }
      }

      return topic != null;
    }

    internal bool TryBuildQuery(out QueryType query)
    {
      query = null;

      if(_info.IsQuery)
      {
        if(TryGetConstructor(out var constructor))
        {
          query = new QueryType(_info, constructor, new FlowObservationSet(), GetResumeAlgorithm(), GetQueryBatchSize());

          DeclareObservations(query);
        }
      }

      return query != null;
    }

    bool TryGetConstructor(out FlowConstructor constructor)
    {
      constructor = null;

      var constructors = _info.DeclaredType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

      if(constructors.Length == 0)
      {
        Log.Warning("[timeline] Flow {Type} has no public parameterless constructor; ignoring", _info);
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
            Log.Warning("[timeline] Flow {Type} constructor has parameters; ignoring", _info);
          }
        }
      }

      return constructor != null;
    }

    ResumeAlgorithm GetResumeAlgorithm() =>
      _info.DeclaredType.GetCustomAttribute<ResumeAlgorithmAttribute>()?.Algorithm ?? new ResumeAlgorithm();

    int GetQueryBatchSize() =>
      _info.DeclaredType.GetCustomAttribute<QueryBatchSizeAttribute>(inherit: true).BatchSize;

    void DeclareObservations(FlowType flow) =>
      new FlowObservationBuilder(_map, flow).Build();
  }
}