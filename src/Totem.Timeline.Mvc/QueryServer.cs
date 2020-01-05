using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// A server for GET requests to queries
  /// </summary>
  public sealed class QueryServer : IQueryServer
  {
    readonly IQueryDb _db;
    readonly IHttpContextAccessor _httpContextAccessor;

    public QueryServer(IQueryDb db, IHttpContextAccessor httpContextAccessor)
    {
      _db = db;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Get(Type type, Id id)
    {
      var content = await _db.ReadQueryContent(GetETag(), type, id);

      return content.NotModified
        ? new QueryNotModifiedResult(content.ETag)
        : new QueryContentResult(content) as IActionResult;
    }

    string GetETag() =>
      _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.IfNoneMatch];
  }
}