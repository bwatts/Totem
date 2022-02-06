using Totem.Core;
using Totem.Local;

namespace Totem;

public interface ILocalQueryContext<out TQuery> : IQueryContext<TQuery>
    where TQuery : ILocalQuery
{
    new ILocalQueryEnvelope Envelope { get; }
    new LocalQueryInfo Info { get; }
}
