namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing a topic on the timeline
  /// </summary>
  public sealed class TopicType : FlowType
  {
    public TopicType(MapTypeInfo info, FlowConstructor constructor, ResumeAlgorithm resumeAlgorithm)
      : base(info, constructor, resumeAlgorithm)
    {}
  }
}