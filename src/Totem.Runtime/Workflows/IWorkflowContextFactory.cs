using Totem.Core;

namespace Totem.Workflows;

public interface IWorkflowContextFactory
{
    IWorkflowContext<IEvent> Create(Id pipelineId, IEventContext<IEvent> eventContext, ItemKey workflowKey);
}
