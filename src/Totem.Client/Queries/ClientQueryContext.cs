using System.Net.Http.Headers;
using Totem.Core;
using Totem.Http;

namespace Totem.Queries;

public class ClientQueryContext<TQuery> : MessageContext, IClientQueryContext<TQuery>
    where TQuery : IHttpQuery
{
    public ClientQueryContext(Id pipelineId, IHttpQueryEnvelope envelope) : base(pipelineId, envelope)
    {
        Envelope = envelope;

        if(envelope.Message is not TQuery query)
            throw new ArgumentException($"Expected query type {typeof(TQuery)} but received {envelope.MessageKey.DeclaredType}", nameof(envelope));

        Query = query;
    }

    public new IHttpQueryEnvelope Envelope { get; }
    public TQuery Query { get; }
    public Type QueryType => Envelope.MessageKey.DeclaredType;
    public Id QueryId => Envelope.MessageKey.Id;
    public Type ResultType => Envelope.Info.Result.DeclaredType;
    public object? Result { get; set; }
    public string Accept { get; set; } = ContentTypes.Json;
    public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
    public ClientResponse? Response { get; set; }

    public T ResultAs<T>()
    {
        if(Result is null)
            throw new InvalidOperationException("Result has not been set on this context");

        if(!ResultType.IsAssignableFrom(typeof(T)))
            throw new ArgumentOutOfRangeException(nameof(T), $"Expected result type {typeof(T)} to be assignabled to {ResultType}");

        if(Result is not T result)
            throw new ArgumentOutOfRangeException(nameof(T), $"Expected result of type {typeof(T)} but received {Result.GetType()}");

        return result;
    }
}
