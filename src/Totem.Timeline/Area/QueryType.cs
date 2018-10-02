namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing a query on the timeline
  /// </summary>
  public sealed class QueryType : FlowType
  {
    public QueryType(MapTypeInfo info, FlowConstructor constructor, ResumeAlgorithm resumeAlgorithm, int batchSize)
      : base(info, constructor, resumeAlgorithm)
    {
      BatchSize = batchSize;
    }

    public readonly int BatchSize;

    public const int DefaultBatchSize = 200;
  }
}