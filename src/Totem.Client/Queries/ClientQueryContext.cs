using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Totem.Core;
using Totem.Http;

namespace Totem.Queries
{
    public class ClientQueryContext<TQuery> : MessageContext, IClientQueryContext<TQuery>
        where TQuery : IQuery
    {
        public ClientQueryContext(Id pipelineId, IQueryEnvelope envelope) : base(pipelineId, envelope)
        {
            Envelope = envelope;

            if(envelope.Message is not TQuery query)
                throw new ArgumentException($"Expected query type {typeof(TQuery)} but received {envelope.MessageType}", nameof(envelope));

            Query = query;
        }

        public new IQueryEnvelope Envelope { get; }
        public TQuery Query { get; }
        public Type QueryType => Envelope.MessageType;
        public Id QueryId => Envelope.MessageId;
        public Type ResultType => Envelope.ResultType;
        public object? Result { get; set; }
        public string Accept { get; set; } = ContentTypes.Json;
        public HttpRequestHeaders Headers { get; } = new HttpRequestMessage().Headers;
        public ClientResponse? Response { get; set; }

        public T ResultAs<T>()
        {
            if(Result == null)
                throw new InvalidOperationException("Result has not been set on this context");

            if(!ResultType.IsAssignableFrom(typeof(T)))
                throw new ArgumentOutOfRangeException(nameof(T), $"Expected result type {typeof(T)} to be assignabled to {ResultType}");

            if(Result is not T result)
                throw new ArgumentOutOfRangeException(nameof(T), $"Expected result of type {typeof(T)} but received {Result.GetType()}");

            return result;
        }
    }
}