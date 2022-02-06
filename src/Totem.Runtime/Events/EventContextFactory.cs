using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Totem.Core;
using Totem.Map;

namespace Totem.Events;

public class EventContextFactory : IEventContextFactory
{
    delegate IEventContext<IEvent> TypeFactory(Id pipelineId, IEventEnvelope envelope);

    readonly ConcurrentDictionary<EventType, TypeFactory> _factoriesByEventType = new();
    readonly RuntimeMap _map;

    public EventContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public IEventContext<IEvent> Create(Id pipelineId, IEventEnvelope envelope)
    {
        if(envelope is null)
            throw new ArgumentNullException(nameof(envelope));

        if(!_map.Events.TryGet(envelope.MessageKey.DeclaredType, out var eventType))
            throw new ArgumentException($"Expected known event of type {envelope.MessageKey.DeclaredType}", nameof(envelope));

        var factory = _factoriesByEventType.GetOrAdd(eventType, CompileFactory);

        return factory(pipelineId, envelope);
    }

    TypeFactory CompileFactory(EventType eventType)
    {
        // (pipelineId, envelope) => new EventContext<TEvent>(pipelineId, envelope, eventType)

        var pipelineIdParameter = Expression.Parameter(typeof(Id), "pipelineId");
        var envelopeParameter = Expression.Parameter(typeof(IEventEnvelope), "envelope");
        var constructor = typeof(EventContext<>)
            .MakeGenericType(eventType.DeclaredType)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single();

        var lambda = Expression.Lambda<TypeFactory>(
            Expression.New(constructor, pipelineIdParameter, envelopeParameter, Expression.Constant(eventType)),
            pipelineIdParameter,
            envelopeParameter);

        return lambda.Compile();
    }
}
