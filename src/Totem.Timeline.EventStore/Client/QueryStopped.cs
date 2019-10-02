namespace Totem.Timeline.EventStore.Client
{
  /// <summary>
  /// Indicates a query stopped and has a new ETag
  /// </summary>
  public sealed class QueryStopped : Event
  {
    public QueryStopped(string etag, string error)
    {
      ETag = etag;
      Error = error;
    }

    public readonly string ETag;
    public readonly string Error;
  }
}