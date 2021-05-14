using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Totem.Core;

namespace Totem
{
    public interface IQueryContext<out TQuery> : IMessageContext
        where TQuery : IQuery
    {
        TQuery Query { get; }
        Type QueryType { get; }
        Id QueryId { get; }
        ClaimsPrincipal? User { get; set; }
        HttpStatusCode ResponseCode { get; set; }
        NameValueCollection ResponseHeaders { get; }
        string? ResponseContentType { get; set; }
        object? ResponseContent { get; set; }
    }
}