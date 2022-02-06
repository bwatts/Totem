using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Workflows;

public interface IWorkflowPipeline
{
    Id Id { get; }

    Task<IWorkflowContext<IEvent>> RunAsync(IEventContext<IEvent> eventContext, ItemKey workflowKey, CancellationToken cancellationToken);
}
