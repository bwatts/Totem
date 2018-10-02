using System.IO;
using System.Threading;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// The state of the most recent version of a query
  /// </summary>
  public class QueryState
  {
    Stream _content;

    QueryState(QueryETag etag, bool notFound = false, bool notModified = false, Stream content = null)
    {
      ETag = etag;
      NotFound = notFound;
      NotModified = notModified;
      _content = content;
    }

    public readonly QueryETag ETag;
    public readonly bool NotFound;
    public readonly bool NotModified;

    public override string ToString() =>
      ETag.ToString();

    public Stream ReadContent()
    {
      Expect.False(NotFound, "Cannot read content of a query that was not found");
      Expect.False(NotModified, "Cannot read content of a query that was not modified");
      Expect.That(_content).IsNotNull("Cannot read content of a query more than once");

      return Interlocked.Exchange(ref _content, null);
    }

    public static QueryState OfNotFound(QueryETag etag) =>
      new QueryState(etag, notFound: true);

    public static QueryState OfNotModified(QueryETag etag) =>
      new QueryState(etag, notModified: true);

    public static QueryState OfContent(QueryETag etag, Stream content)
    {
      Expect.True(etag.Checkpoint.IsSome, "A query without a checkpoint cannot have content");

      return new QueryState(etag, content: content);
    }
  }
}