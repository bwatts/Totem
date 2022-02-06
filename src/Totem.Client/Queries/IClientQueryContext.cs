using System.Net.Http.Headers;
using Totem.Core;
using Totem.Http;

namespace Totem.Queries;

public interface IClientQueryContext<out TQuery> : IMessageContext
    where TQuery : IHttpQuery
{
    TQuery Query { get; }
    Type QueryType { get; }
    Id QueryId { get; }
    Type ResultType { get; }
    object? Result { get; set; }
    string Accept { get; set; }
    HttpRequestHeaders Headers { get; }
    ClientResponse? Response { get; set; }
}
