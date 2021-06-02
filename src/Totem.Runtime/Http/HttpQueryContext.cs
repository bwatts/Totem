using System;
using System.Collections.Specialized;
using System.Net;
using Totem.Core;

namespace Totem.Http
{
    public class HttpQueryContext<TQuery> : MessageContext, IHttpQueryContext<TQuery>
        where TQuery : IHttpQuery
    {
        public HttpQueryContext(Id pipelineId, IHttpQueryEnvelope envelope) : base(pipelineId, envelope)
        {
            if(envelope.Message is not TQuery query)
                throw new ArgumentException($"Expected query type {typeof(TQuery)} but received {envelope.Info.MessageType}", nameof(envelope));

            Query = query;
        }

        public new IHttpQueryEnvelope Envelope => (IHttpQueryEnvelope) base.Envelope;
        public new HttpQueryInfo Info => (HttpQueryInfo) base.Info;
        public TQuery Query { get; }
        public Type QueryType => Envelope.Info.MessageType;
        public Id QueryId => Envelope.MessageId;
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;
        public NameValueCollection ResponseHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string? ResponseContentType { get; set; }
        public object? ResponseContent { get; set; }
    }
}