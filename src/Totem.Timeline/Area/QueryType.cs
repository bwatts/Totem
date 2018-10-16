namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing a query on the timeline
  /// </summary>
  public class QueryType : FlowType
  {
    public QueryType(FlowTypeInfo info, int batchSize) : base(info)
    {
      BatchSize = batchSize;
    }

    public readonly int BatchSize;

    public const int DefaultBatchSize = 200;
  }
}