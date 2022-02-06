using System.Diagnostics.CodeAnalysis;
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
    public bool HasHandler => Handler is not null;
    public QueryHandlerType? Handler { get; internal set; }
    public Type? HandlerServiceType { get; internal set; }

    Type ITypeKeyed.TypeKey => ContextType;

    public override string ToString() =>
        ContextType.ToString();
}
