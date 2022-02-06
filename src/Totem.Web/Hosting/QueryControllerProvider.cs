using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Totem.Http;
using Totem.Map;

namespace Totem.Hosting;

public class QueryControllerProvider : IApplicationFeatureProvider<ControllerFeature>
{
    readonly RuntimeMap _map;

    public QueryControllerProvider(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        if(parts is null)
            throw new ArgumentNullException(nameof(parts));

        if(feature is null)
            throw new ArgumentNullException(nameof(feature));

        foreach(var controllerType in
            from query in _map.Queries
            from context in query.Contexts
            where context.Info is HttpQueryInfo
            select typeof(HttpQueryController<>).MakeGenericType(query.DeclaredType))
        {
            feature.Controllers.Add(controllerType.GetTypeInfo());
        }
    }
}
