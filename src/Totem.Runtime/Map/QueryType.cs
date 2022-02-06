using System;

namespace Totem.Map;

public class QueryType : MessageType
{
    internal QueryType(Type declaredType) : base(declaredType)
    { }

    public TypeKeyedCollection<QueryContext> Contexts { get; } = new();
}
