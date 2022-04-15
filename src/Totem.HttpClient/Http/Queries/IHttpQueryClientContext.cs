namespace Totem.Http.Queries;

public interface IHttpQueryClientContext<out TQuery> : IMessageContext
    where TQuery : IHttpQuery
{
    new IHttpQueryEnvelope Envelope { get; }
    new HttpQueryInfo Info { get; }
    TQuery Query { get; }
    ItemKey QueryKey { get; }
    Type QueryType { get; }
    Id QueryId { get; }
    Type ResultType { get; }
    object? Result { get; set; }
    string Accept { get; set; }
    HttpRequestHeaders Headers { get; }
    ClientResponse? Response { get; set; }
}
