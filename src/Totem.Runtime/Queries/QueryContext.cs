using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using Totem.Core;

namespace Totem.Queries
{
    public class QueryContext<TQuery> : MessageContext, IQueryContext<TQuery>
        where TQuery : IQuery
    {
        public QueryContext(Id pipelineId, IQueryEnvelope envelope) : base(pipelineId, envelope)
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
        public ClaimsPrincipal? User { get; set; }
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
        public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string? ResponseContentType { get; set; }
        public object? ResponseContent { get; set; }
    }
}