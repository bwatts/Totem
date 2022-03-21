using Totem.Core;

namespace Totem.Map.Builder;

internal class MapBuilder
{
    readonly RuntimeMap _map;
    readonly IEnumerable<Type> _types;

    internal MapBuilder(RuntimeMap map, IEnumerable<Type> types)
    {
        _map = map;
        _types = types;
    }

    internal void Build()
    {
        foreach(var type in _types)
        {
            TryAdd(
                type,
                TryAddEventHandler,
                TryAddHttpQueryHandler,
                TryAddLocalQueryHandler,
                type => _map.TryAddReport(type),
                type => _map.TryAddTopic(type),
                type => _map.TryAddWorkflow(type));
        }
    }

    static bool TryAdd(Type type, params Func<Type, bool>[] tryAdds)
    {
        foreach(var tryAdd in tryAdds)
        {
            if(tryAdd(type))
            {
                return true;
            }
        }

        return false;
    }

    bool TryAddEventHandler(Type type)
    {
        var eventType = type.GetImplementedInterfaceGenericArguments(typeof(IEventHandler<>)).FirstOrDefault();

        if(eventType is null)
        {
            return false;
        }

        var e = _map.GetOrAddEvent(eventType);
        var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
        var handler = new EventHandlerType(type, e, handlerType);

        _map.EventHandlers.Add(handler);
        e.Handlers.Add(handler);

        return true;
    }

    bool TryAddHttpQueryHandler(Type type) =>
        TryAddQueryHandler(type, typeof(IHttpQueryContext<>), typeof(IHttpQueryHandler<>));

    bool TryAddLocalQueryHandler(Type type) =>
        TryAddQueryHandler(type, typeof(ILocalQueryContext<>), typeof(ILocalQueryHandler<>));

    bool TryAddQueryHandler(Type type, Type contextInterface, Type handlerInterface)
    {
        var queryType = type.GetImplementedInterfaceGenericArguments(handlerInterface).FirstOrDefault();

        if(queryType is null)
        {
            return false;
        }

        var query = _map.GetOrAddQuery(queryType);
        var contextType = contextInterface.MakeGenericType(queryType);

        if(query.Contexts.TryGet(contextType, out var context))
        {
            if(!_map.QueryHandlers.TryGet(type, out var handler))
            {
                handler = new QueryHandlerType(type, query);

                _map.QueryHandlers.Add(handler);
            }

            var serviceType = handlerInterface.MakeGenericType(queryType);

            context.Handler = handler;
            context.HandlerServiceType = serviceType;

            handler.AddServiceType(serviceType);
            return true;
        }

        return false;
    }
}
