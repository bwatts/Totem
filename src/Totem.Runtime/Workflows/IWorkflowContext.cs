using Totem.Core;
using Totem.Map;

namespace Totem.Workflows;

public interface IWorkflowContext<out TEvent> where TEvent : IEvent
{
    Id PipelineId { get; }
    IEventContext<TEvent> EventContext { get; }
    ItemKey WorkflowKey { get; }
    WorkflowType WorkflowType { get; }
}
