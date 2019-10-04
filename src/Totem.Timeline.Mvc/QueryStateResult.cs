using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Indicates a request for the latest version of a query
  /// </summary>
  public class QueryStateResult : ActionResult
  {
    public QueryStateResult(QueryState state)
    {
      State = state;
    }

    public readonly QueryState State;

    public override async Task ExecuteResultAsync(ActionContext context)
    {
      context.HttpContext.Response.StatusCode = 200;
      context.HttpContext.Response.Headers[HeaderNames.ETag] = $"\"{State.ETag.ToString()}\"";

      await State
        .ReadContent()
        .CopyToAsync(context.HttpContext.Response.Body);
    }
  }
}