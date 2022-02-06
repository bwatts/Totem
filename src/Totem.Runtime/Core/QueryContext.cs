using Totem.Map;

namespace Totem.Core;

public abstract class QueryContext<TQuery> : MessageContext, IQueryContext<TQuery>
    where TQuery : IQueryMessage
{
    public QueryContext(Id pipelineId, IQueryEnvelope envelope, QueryType queryType) : base(pipelineId, envelope) =>
        QueryType = queryType;

    public new IQueryEnvelope Envelope => (IQueryEnvelope) base.Envelope;
    public new QueryInfo Info => (QueryInfo) base.Info;
    public abstract Type InterfaceType { get; }
    public TQuery Query => (TQuery) Envelope.Message;
    public ItemKey QueryKey => Envelope.MessageKey;
    public QueryType QueryType { get; }
    public Id QueryId => Envelope.MessageKey.Id;
    public Type ResultType => Info.Result.DeclaredType;
    public object? Result { get; set; }

    QueryInfo IQueryContext<TQuery>.Info => Info;
}
