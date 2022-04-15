using Totem.Core;

namespace Totem.Map;

public class QueryContext : ITypeKeyed
{
    delegate Task CallHandler(IQueryContext<IQueryMessage> context, IServiceProvider services, CancellationToken cancellationToken);

    internal QueryContext(Type contextType, QueryInfo info)
    {
        ContextType = contextType;
        Info = info;
    }

    public Type ContextType { get; }
    public QueryInfo Info { get; }
    [MemberNotNullWhen(true, nameof(Handler), nameof(HandlerServiceType))]
    public QueryHandlerType Handler { get; internal set; } = null!;
    public Type HandlerServiceType { get; internal set; } = null!;

    Type ITypeKeyed.TypeKey => ContextType;

    public override string ToString() =>
        ContextType.ToString();
}
