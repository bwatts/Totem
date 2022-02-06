using Totem.Map;

namespace Totem.Core;

public interface IQueryContext<out TQuery> : IMessageContext
    where TQuery : IQueryMessage
{
    new QueryInfo Info { get; }
    new IQueryEnvelope Envelope { get; }
    Type InterfaceType { get; }
    TQuery Query { get; }
    ItemKey QueryKey { get; }
    QueryType QueryType { get; }
    Id QueryId { get; }
    Type ResultType { get; }
    object? Result { get; set; }
}
