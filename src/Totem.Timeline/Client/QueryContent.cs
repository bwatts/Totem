using System.IO;
using System.Threading;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// The data associated  of the most recent version of a query
  /// </summary>
  public class QueryContent
  {
    Stream _data;

    public QueryContent(QueryETag etag)
    {
      ETag = etag;
      NotModified = true;
    }

    public QueryContent(QueryETag etag, Stream content)
    {
      ETag = etag;
      _data = content;
    }

    public readonly QueryETag ETag;
    public readonly bool NotModified;

    public override string ToString() =>
      ETag.ToString();

    public Stream ReadData()
    {
      Expect.That(NotModified).IsFalse("Cannot read data of a query that was not modified");
      Expect.That(_data).IsNotNull("Cannot read data of a query more than once");

      return Interlocked.Exchange(ref _data, null);
    }
  }
}