using Totem.Core;
using Totem.Map;

namespace Totem.Local;

public class LocalQueryContext<TQuery> : QueryContext<TQuery>, ILocalQueryContext<TQuery>
    where TQuery : ILocalQuery
{
    internal LocalQueryContext(Id pipelineId, ILocalQueryEnvelope envelope, QueryType queryType) : base(pipelineId, envelope, queryType)
    { }

    public new ILocalQueryEnvelope Envelope => (ILocalQueryEnvelope) base.Envelope;
    public new LocalQueryInfo Info => (LocalQueryInfo) base.Info;
    public override Type InterfaceType => typeof(ILocalQueryContext<TQuery>);
}
