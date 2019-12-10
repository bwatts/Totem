using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Totem.IO;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Indicates a request for the latest version of a query
  /// </summary>
  public class QueryContentResult : ActionResult
  {
    public QueryContentResult(QueryContent content)
    {
      Content = content;
    }

    public readonly QueryContent Content;

    public override async Task ExecuteResultAsync(ActionContext context)
    {
      context.HttpContext.Response.StatusCode = 200;
      context.HttpContext.Response.Headers[HeaderNames.ETag] = Content.ETag.ToString();
      context.HttpContext.Response.ContentType = MediaType.Json.ToString();

      await Content
        .ReadData()
        .CopyToAsync(context.HttpContext.Response.Body);
    }
  }
}