using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Http;

namespace Totem;

public interface IHttpQueryContext<out TQuery> : IQueryContext<TQuery>
    where TQuery : IHttpQuery
{
    new IHttpQueryEnvelope Envelope { get; }
    new HttpQueryInfo Info { get; }
    HttpStatusCode ResponseCode { get; set; }
    NameValueCollection ResponseHeaders { get; }
    string? ResponseContentType { get; set; }
}
