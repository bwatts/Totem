using System;
using System.Collections.Generic;

namespace Totem.Map;

public class QueryHandlerType : MapType
{
    readonly List<Type> _serviceTypes = new();

    internal QueryHandlerType(Type declaredType, QueryType query) : base(declaredType) =>
        Query = query;

    public QueryType Query { get; }
    public IReadOnlyCollection<Type> ServiceTypes => _serviceTypes;

    internal void AddServiceType(Type serviceType) =>
        _serviceTypes.Add(serviceType);
}
