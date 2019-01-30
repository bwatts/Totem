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

    public QueryState(QueryETag etag)
    {
      ETag = etag;
      NotModified = true;
    }

    public QueryState(QueryETag etag, Stream content)
    {
      ETag = etag;
      _content = content;
    }

    public readonly QueryETag ETag;
    public readonly bool NotModified;

    public override string ToString() =>
      ETag.ToString();

    public Stream ReadContent()
    {
      Expect.False(NotModified, "Cannot read content of a query that was not modified");
      Expect.That(_content).IsNotNull("Cannot read content of a query more than once");

      return Interlocked.Exchange(ref _content, null);
    }
  }
}