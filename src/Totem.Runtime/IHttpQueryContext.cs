using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;
using Totem.Http;

namespace Totem
{
    public interface IHttpQueryContext<out TQuery> : IMessageContext
        where TQuery : IHttpQuery
    {
        new IHttpQueryEnvelope Envelope { get; }
        new HttpQueryInfo Info { get; }
        TQuery Query { get; }
        Type QueryType { get; }
        Id QueryId { get; }
        HttpStatusCode ResponseCode { get; set; }
        NameValueCollection ResponseHeaders { get; }
        string? ResponseContentType { get; set; }
        object? ResponseContent { get; set; }
    }
}