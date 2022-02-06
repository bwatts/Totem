using Totem.Core;
using Totem.Http;
using Totem.Local;
using Totem.Queues;

namespace Totem.Map.Builder;

internal class MapBuilder
{
    readonly IEnumerable<Type> _types;
    readonly RuntimeMap _map = new();

    internal MapBuilder(IEnumerable<Type> types) =>
        _types = types;

    internal RuntimeMap Build()
    {
        var nonMessages = new List<Type>();

        foreach(var type in _types)
        {
            if(!TryAdd(type, TryAddCommandContexts, TryAddQueryContexts, TryAddEvent))
            {
                nonMessages.Add(type);
            }
        }

        foreach(var type in nonMessages)
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

        return _map;
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

    bool TryAddCommandContexts(Type declaredType)
    {
        var added = false;

        if(HttpCommandInfo.TryFrom(declaredType, out var httpInfo))
        {
            AddCommand(declaredType, httpInfo, typeof(IHttpCommandContext<>));
            added = true;
        }

        if(LocalCommandInfo.TryFrom(declaredType, out var localInfo))
        {
            AddCommand(declaredType, localInfo, typeof(ILocalCommandContext<>));
            added = true;
        }

        if(QueueCommandInfo.TryFrom(declaredType, out var queueInfo))
        {
            AddCommand(declaredType, queueInfo, typeof(IQueueCommandContext<>));
            added = true;
        }

        return added;
    }

    bool TryAddQueryContexts(Type declaredType)
    {
        var added = false;

        if(HttpQueryInfo.TryFrom(declaredType, out var httpInfo))
        {
            AddQuery(declaredType, httpInfo, typeof(IHttpQueryContext<>));
            added = true;
        }

        if(LocalQueryInfo.TryFrom(declaredType, out var localInfo))
        {
            AddQuery(declaredType, localInfo, typeof(ILocalQueryContext<>));
            added = true;
        }

        return added;
    }

    bool TryAddEvent(Type declaredType)
    {
        if(EventInfo.TryFrom(declaredType, out var info))
        {
            _map.Events.Add(new EventType(info));
            return true;
        }

        return false;
    }

    void AddCommand(Type declaredType, CommandInfo info, Type contextType)
    {
        if(!_map.Commands.TryGet(declaredType, out var command))
        {
            command = new(declaredType);

            _map.Commands.Add(command);
        }

        command.Contexts.Add(new TopicCommandContext(contextType.MakeGenericType(declaredType), info));
    }

    void AddQuery(Type declaredType, QueryInfo info, Type contextType)
    {
        if(!_map.Queries.TryGet(declaredType, out var query))
        {
            query = new(declaredType);

            _map.Queries.Add(query);
        }

        query.Contexts.Add(new QueryContext(contextType.MakeGenericType(declaredType), info));
    }

    bool TryAddEventHandler(Type type)
    {
        var eventType = type.GetImplementedInterfaceGenericArguments(typeof(IEventHandler<>)).FirstOrDefault();

        if(eventType is not null && _map.Events.TryGet(eventType, out var e))
        {
            var handler = new EventHandlerType(type, e, typeof(IEventHandler<>).MakeGenericType(type));

            _map.EventHandlers.Add(handler);

            e.Handlers.Add(handler);

            return true;
        }

        return false;
    }

    bool TryAddHttpQueryHandler(Type type) =>
        TryAddQueryHandler(type, typeof(IHttpQueryContext<>), typeof(IHttpQueryHandler<>));

    bool TryAddLocalQueryHandler(Type type) =>
        TryAddQueryHandler(type, typeof(ILocalQueryContext<>), typeof(ILocalQueryHandler<>));

    bool TryAddQueryHandler(Type type, Type contextInterface, Type handlerInterface)
    {
        var queryType = type.GetImplementedInterfaceGenericArguments(handlerInterface).FirstOrDefault();

        if(queryType is null || !_map.Queries.TryGet(queryType, out var query))
        {
            return false;
        }

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
