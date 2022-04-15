using Totem.Core;
using Totem.Http;
using Totem.Local;

namespace Totem.Map.Builder;

internal static class QueryReflection
{
    internal static bool TryAddQueryHandler(this RuntimeMap map, Type declaredType) =>
        map.TryAddQueryHandler(declaredType, typeof(IHttpQueryHandler<>), typeof(IHttpQueryContext<>), HttpQueryInfo.From)
        || map.TryAddQueryHandler(declaredType, typeof(ILocalQueryHandler<>), typeof(ILocalQueryContext<>), LocalQueryInfo.From);

    static bool TryAddQueryHandler(
        this RuntimeMap map,
        Type declaredType,
        Type handlerInterface,
        Type contextInterface,
        Func<Type, QueryInfo> infoFrom)
    {
        var queryType = declaredType.GetImplementedInterfaceGenericArguments(handlerInterface).FirstOrDefault();

        if(queryType is null)
        {
            return false;
        }

        var info = infoFrom(queryType);
        var contextType = contextInterface.MakeGenericType(declaredType);
        var context = new QueryContext(contextType, info);
        var query = map.GetOrAddQuery(queryType);

        query.Contexts.Add(context);

        if(!map.QueryHandlers.TryGet(declaredType, out var handler))
        {
            handler = new QueryHandlerType(declaredType, query);

            map.QueryHandlers.Add(handler);
        }

        var serviceType = handlerInterface.MakeGenericType(queryType);

        context.Handler = handler;
        context.HandlerServiceType = serviceType;

        handler.AddServiceType(serviceType);

        return true;
    }
}
