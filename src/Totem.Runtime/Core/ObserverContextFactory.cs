using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Map;

namespace Totem.Core;

internal class ObserverContextFactory<TContext>
{
    delegate TContext EventTypeFactory(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey workflowKey);

    readonly ConcurrentDictionary<EventType, EventTypeFactory> _factoriesByEventType = new();
    readonly ObserverType _observerType;
    readonly Type _genericContextType;

    internal ObserverContextFactory(ObserverType observerType, Type genericContextType)
    {
        _observerType = observerType;
        _genericContextType = genericContextType;
    }

    internal TContext Create(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey observerKey)
    {
        var create = _factoriesByEventType.GetOrAdd(eventContext.EventType, CompileFactory);

        return create(pipelineId, eventContext, observerKey);
    }

    EventTypeFactory CompileFactory(EventType eventType)
    {
        // (pipelineId, eventContext, workflowKey) =>
        //     new TContext<TEvent>(pipelineId, (IEventContext<TEvent>) eventContext, observerKey, _observerType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var eventContextParameter = Expression.Parameter(typeof(IEventContext<IEvent>), "eventContext");
        var eventContextCast = Expression.Convert(eventContextParameter, typeof(IEventContext<>).MakeGenericType(eventType.DeclaredType));
        var observerKeyParameter = Expression.Parameter(typeof(ItemKey), "observerKey");
        var constructor = _genericContextType
            .MakeGenericType(eventType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<EventTypeFactory>(
            Expression.New(
                constructor,
                pipelineIdParameter,
                eventContextCast,
                observerKeyParameter,
                Expression.Constant(_observerType)),
            pipelineIdParameter,
            eventContextParameter,
            observerKeyParameter);

        return lambda.Compile();
    }
}
