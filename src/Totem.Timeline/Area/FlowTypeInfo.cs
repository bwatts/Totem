using System;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A description of a .NET type representing a flow in a timeline map
  /// </summary>
  public class FlowTypeInfo : MapTypeInfo
  {
    public FlowTypeInfo(
      AreaKey area,
      Type declaredType,
      FlowConstructor constructor,
      FlowObservationSet observations,
      ResumeAlgorithm resumeAlgorithm)
      : base(area, declaredType)
    {
      Constructor = constructor;
      Observations = observations;
      ResumeAlgorithm = resumeAlgorithm;
    }

    public readonly FlowConstructor Constructor;
    public readonly FlowObservationSet Observations;
    public readonly ResumeAlgorithm ResumeAlgorithm;

    public bool IsTopic => typeof(Topic).IsAssignableFrom(DeclaredType);
    public bool IsQuery => typeof(Query).IsAssignableFrom(DeclaredType);
  }
}