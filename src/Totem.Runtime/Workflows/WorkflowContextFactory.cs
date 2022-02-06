using System;
using System.Collections.Concurrent;
using Totem.Core;
using Totem.Map;

namespace Totem.Workflows;

public class WorkflowContextFactory : IWorkflowContextFactory
{
    readonly ConcurrentDictionary<Type, ObserverContextFactory<IWorkflowContext<IEvent>>> _factoriesByWorkflowType = new();
    readonly RuntimeMap _map;

    public WorkflowContextFactory(RuntimeMap map) =>
        _map = map ?? throw new ArgumentNullException(nameof(map));

    public IWorkflowContext<IEvent> Create(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey workflowKey)
    {
        if(eventContext is null)
            throw new ArgumentNullException(nameof(eventContext));

        if(workflowKey is null)
            throw new ArgumentNullException(nameof(workflowKey));

        var factory = _factoriesByWorkflowType.GetOrAdd(
            workflowKey.DeclaredType,
            type => new(_map.Workflows[workflowKey.DeclaredType], typeof(WorkflowContext<>)));

        return factory.Create(pipelineId, eventContext, workflowKey);
    }
}
