namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declaring a topic in a timeline area
  /// </summary>
  public sealed class TopicType : FlowType
  {
    internal TopicType(
      AreaTypeInfo info,
      FlowConstructor constructor,
      FlowObservationSet observations,
      ResumeAlgorithm resumeAlgorithm)
      : base(info, constructor, observations, resumeAlgorithm)
    {}
  }
}