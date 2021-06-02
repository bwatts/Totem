using System;
using Totem.Core;
using Totem.Local;

namespace Totem
{
    public interface ILocalQueryContext<out TQuery> : IMessageContext
        where TQuery : ILocalQuery
    {
        new ILocalQueryEnvelope Envelope { get; }
        new LocalQueryInfo Info { get; }
        TQuery Query { get; }
        Type QueryType { get; }
        Id QueryId { get; }
        Type ResultType { get; }
        object? Result { get; set; }
    }
}