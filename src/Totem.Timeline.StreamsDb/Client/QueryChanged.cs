namespace Totem.Timeline.StreamsDb.Client
{
  /// <summary>
  /// Indicates a query changed its state and has a new ETag
  /// </summary>
  public sealed class QueryChanged : Event
  {
    public QueryChanged(string etag)
    {
      ETag = etag;
    }

    public readonly string ETag;
  }
}