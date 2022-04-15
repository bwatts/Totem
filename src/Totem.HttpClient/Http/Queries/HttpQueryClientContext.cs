namespace Totem.Http.Queries;

public class HttpQueryClientContext<TQuery> : MessageContext, IHttpQueryClientContext<TQuery>
    where TQuery : IHttpQuery
{
    public HttpQueryClientContext(Id pipelineId, IHttpQueryEnvelope envelope) : base(pipelineId, envelope)
    { }

    public new IHttpQueryEnvelope Envelope => (IHttpQueryEnvelope) base.Envelope;
    public new HttpQueryInfo Info => (HttpQueryInfo) base.Info;
    public TQuery Query => (TQuery) Envelope.Message;
    public ItemKey QueryKey => Envelope.MessageKey;
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
            throw new InvalidCastException($"Expected result of type {typeof(T)} but received {Result.GetType()}");

        return result;
    }
}
