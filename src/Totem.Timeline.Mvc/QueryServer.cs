using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Totem.Timeline.Area;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// A server for GET requests to queries
  /// </summary>
  public sealed class QueryServer : IQueryServer
  {
    readonly AreaMap _area;
    readonly IQueryDb _db;
    readonly IHttpContextAccessor _httpContextAccessor;

    public QueryServer(AreaMap area, IQueryDb db, IHttpContextAccessor httpContextAccessor)
    {
      _area = area;
      _db = db;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Get(Type type, Id id)
    {
      var etag = ReadETag(type, id);

      var state = await _db.ReadState(etag);

      return state.NotModified
        ? new QueryNotModifiedResult(etag)
        : new QueryStateResult(state) as IActionResult;
    }

    QueryETag ReadETag(Type type, Id id)
    {
      if(TryGetIfNoneMatch(out var ifNoneMatch))
      {
        return QueryETag.From(ifNoneMatch, _area);
      }

      var key = FlowKey.From(_area.Queries.Get(type), id);

      return QueryETag.From(key, TimelinePosition.None);
    }

    bool TryGetIfNoneMatch(out StringValues header) =>
      _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out header);
  }
}