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
                type => _map.TryAddQueryHandler(type),
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
}
