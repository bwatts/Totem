using System.Collections.Concurrent;
using Totem.Core;
using Totem.Map;
using Totem.Workflows;

namespace Totem.InMemory;

public class InMemoryWorkflowStore : IWorkflowStore
{
    readonly ConcurrentDictionary<ItemKey, WorkflowHistory> _historiesByKey = new();
    readonly IQueueClient _queueClient;

    public InMemoryWorkflowStore(IQueueClient queueClient) =>
        _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));

    public Task<IWorkflowTransaction> StartTransactionAsync(IWorkflowContext<IEvent> context, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        var workflow = context.WorkflowType.Create(context.WorkflowKey.Id);

        if(_historiesByKey.TryGetValue(context.WorkflowKey, out var history))
        {
            history.Load(workflow);
        }

        context.WorkflowType.CallGivenIfDefined(workflow, context.EventContext.Event);

        return Task.FromResult<IWorkflowTransaction>(new WorkflowTransaction(this, context, workflow, cancellationToken));
    }

    public async Task CommitAsync(IWorkflowTransaction transaction, CancellationToken cancellationToken)
    {
        if(transaction is null)
            throw new ArgumentNullException(nameof(transaction));

        var key = transaction.Context.WorkflowKey;

        _historiesByKey.AddOrUpdate(key, _ => new(transaction), (_, history) => history.Update(transaction));

        await _queueClient.EnqueueAsync(transaction.Workflow.TakeNewCommands(), cancellationToken);
    }

    public Task RollbackAsync(IWorkflowTransaction transaction, CancellationToken cancellationToken)
    {
        // Nothing to do when in memory
        return Task.CompletedTask;
    }

    class WorkflowHistory
    {
        readonly List<IEvent> _events = new();
        readonly WorkflowType _workflowType;

        internal WorkflowHistory(IWorkflowTransaction transaction) =>
            _workflowType = transaction.Context.WorkflowType;

        internal void Load(IWorkflow workflow)
        {
            foreach(var e in _events)
            {
                _workflowType.CallGivenIfDefined(workflow, e);
            }
        }

        internal WorkflowHistory Update(IWorkflowTransaction transaction)
        {
            _events.Add(transaction.Context.EventContext.Event);
            return this;
        }
    }
}
