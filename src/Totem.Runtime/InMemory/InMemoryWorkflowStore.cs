using System.Collections.Concurrent;
using Totem.Core;
using Totem.Map;
using Totem.Workflows;

namespace Totem.InMemory;

public class InMemoryWorkflowStore : IWorkflowStore
{
    readonly ConcurrentDictionary<ItemKey, WorkflowHistory> _historiesByKey = new();
    readonly IStorage _storage;
    readonly IQueueClient _queueClient;

    public InMemoryWorkflowStore(IStorage storage, IQueueClient queueClient)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
    }

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

        await _storage.PutAsync(
            StorageRow.From(key.DeclaredType.FullName!, key.Id.ToString(), transaction.Workflow),
            cancellationToken);

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
        readonly string _workflowId;

        internal WorkflowHistory(IWorkflowTransaction transaction)
        {
            _workflowType = transaction.Context.WorkflowType;
            _workflowId = transaction.Context.WorkflowKey.Id.ToShortString();

            var version = transaction.Workflow.Version ?? 0;

            if(version != 0)
                throw new UnexpectedVersionException($"Expected workflow {_workflowType}/{_workflowId} to not exist, but found @{version}");
        }

        internal void Load(IWorkflow workflow)
        {
            if(workflow.Version is not null)
                throw new InvalidOperationException($"Expected a report with no version, found {workflow}@{workflow.Version}");

            workflow.Version = _events.Count;

            foreach(var e in _events)
            {
                _workflowType.CallGivenIfDefined(workflow, e);
            }
        }

        internal WorkflowHistory Update(IWorkflowTransaction transaction)
        {
            var expectedVersion = _events.Count;

            if(transaction.Workflow.Version != expectedVersion)
                throw new UnexpectedVersionException($"Expected workflow {_workflowType}/{_workflowId}@{expectedVersion}, but received @{transaction.Workflow.Version}");

            _events.Add(transaction.Context.EventContext.Event);

            return this;
        }
    }
}
