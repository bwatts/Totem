namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declaring a query in a timeline area
  /// </summary>
  public sealed class QueryType : FlowType
  {
    internal QueryType(
      AreaTypeInfo info,
      FlowConstructor constructor,
      FlowObservationSet observations,
      ResumeAlgorithm resumeAlgorithm,
      int batchSize)
      : base(info, constructor, observations, resumeAlgorithm)
    {
      BatchSize = batchSize;
    }

    public readonly int BatchSize;

    public const int DefaultBatchSize = 200;
  }
}