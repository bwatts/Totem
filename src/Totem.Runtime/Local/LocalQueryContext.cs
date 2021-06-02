using System;
using Totem.Core;

namespace Totem.Local
{
    public class LocalQueryContext<TQuery> : MessageContext, ILocalQueryContext<TQuery>
        where TQuery : ILocalQuery
    {
        public LocalQueryContext(Id pipelineId, ILocalQueryEnvelope envelope) : base(pipelineId, envelope)
        {
            if(envelope.Message is not TQuery query)
                throw new ArgumentException($"Expected query type {typeof(TQuery)} but received {envelope.Info.MessageType}", nameof(envelope));

            Query = query;
        }

        public new ILocalQueryEnvelope Envelope => (ILocalQueryEnvelope) base.Envelope;
        public new LocalQueryInfo Info => (LocalQueryInfo) base.Info;
        public TQuery Query { get; }
        public Type QueryType => Envelope.Info.MessageType;
        public Id QueryId => Envelope.MessageId;
        public Type ResultType => Info.ResultType;
        public object? Result { get; set; }
    }
}