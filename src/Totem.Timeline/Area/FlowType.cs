namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing a flow on the timeline
  /// </summary>
  public class FlowType : MapType
  {
    public FlowType(MapTypeInfo info, FlowConstructor constructor, ResumeAlgorithm resumeAlgorithm) : base(info)
    {
      Constructor = constructor;
      ResumeAlgorithm = resumeAlgorithm;
      Observations = new FlowObservationSet();

      IsTopic = this is TopicType;
      IsQuery = this is QueryType;
    }

    public readonly FlowConstructor Constructor;
    public readonly ResumeAlgorithm ResumeAlgorithm;
    public readonly FlowObservationSet Observations;
    public readonly bool IsTopic;
    public readonly bool IsQuery;

    public bool IsSingleInstance { get; internal set; }
    public bool IsMultiInstance { get; internal set; }

    public FlowKey CreateKey(Id id)
    {
      Expect.False(IsSingleInstance && id.IsAssigned, $"Flow {this} is single-instance and cannot have an assigned id of {id}");
      Expect.False(IsMultiInstance && id.IsUnassigned, $"Flow {this} is multi-instance and must have an assigned id");

      return FlowKey.From(this, id);
    }

    public Flow New() => Constructor.Call();
  }
}