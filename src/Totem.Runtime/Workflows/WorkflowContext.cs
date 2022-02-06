using Totem.Core;
using Totem.Map;

namespace Totem.Workflows;

public class WorkflowContext<TEvent> : MessageContext, IWorkflowContext<TEvent>
    where TEvent : class, IEvent
{
    internal WorkflowContext(Id pipelineId, IEventContext<TEvent> eventContext, ItemKey workflowKey, WorkflowType workflowType)
        : base(pipelineId, eventContext.Envelope)
    {
        EventContext = eventContext;
        WorkflowKey = workflowKey;
        WorkflowType = workflowType;
    }

    public IEventContext<TEvent> EventContext { get; }
    public ItemKey WorkflowKey { get; }
    public WorkflowType WorkflowType { get; }
}
