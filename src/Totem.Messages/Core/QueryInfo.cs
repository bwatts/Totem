namespace Totem.Core;

public abstract class QueryInfo : MessageInfo
{
    internal QueryInfo(Type declaredType, QueryResult result) : base(declaredType) =>
        Result = result;

    public QueryResult Result { get; }
}
