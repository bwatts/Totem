using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Indicates a request ETag refers to the current version of a query
  /// </summary>
  public class QueryNotFoundResult : ActionResult
  {
    public QueryNotFoundResult(QueryETag etag)
    {
      ETag = etag;
    }

    public readonly QueryETag ETag;

    public override void ExecuteResult(ActionContext context)
    {
      context.HttpContext.Response.StatusCode = 404;
      context.HttpContext.Response.Headers[HeaderNames.ETag] = ETag.ToString();
    }
  }
}