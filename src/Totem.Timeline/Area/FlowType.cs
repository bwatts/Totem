namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declaring a flow in a timeline area
  /// </summary>
  public abstract class FlowType : AreaType
  {
    internal FlowType(
      AreaTypeInfo info,
      FlowConstructor constructor,
      FlowObservationSet observations,
      ResumeAlgorithm resumeAlgorithm)
      : base(info)
    {
      Constructor = constructor;
      Observations = observations;
      ResumeAlgorithm = resumeAlgorithm;
    }

    public readonly FlowConstructor Constructor;
    public readonly FlowObservationSet Observations;
    public readonly ResumeAlgorithm ResumeAlgorithm;

    public bool IsSingleInstance { get; internal set; }
    public bool IsMultiInstance { get; internal set; }

    public Flow New() => Constructor.Call();

    public FlowKey CreateKey(Id id) =>
      FlowKey.From(this, id);

    public void ExpectIdMatchesCardinality(Id id)
    {
      Expect.False(IsSingleInstance && id.IsAssigned, $"Flow {this} is single-instance and cannot have an assigned id of {id}");
      Expect.False(IsMultiInstance && id.IsUnassigned, $"Flow {this} is multi-instance and must have an assigned id");
    }

    public void ExpectObserves(EventType e) =>
      Expect.True(Observations.Contains(e), $"Flow {this} does not observe event {e}");
  }
}