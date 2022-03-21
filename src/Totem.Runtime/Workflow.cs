using System.Collections.Concurrent;
using System.Security.Claims;
using Totem.Core;
using Totem.Queues;
using Totem.Workflows;

namespace Totem;

public abstract class Workflow : Timeline, IWorkflow
{
    public static readonly Id RouteCorrelationId = Id.NewId();

    readonly ConcurrentQueue<IQueueCommandEnvelope> _newCommands = new();

    public bool HasNewCommands =>
        !_newCommands.IsEmpty;

    public IReadOnlyList<IQueueCommandEnvelope> TakeNewCommands()
    {
        var newCommands = new List<IQueueCommandEnvelope>();

        while(_newCommands.TryDequeue(out var newCommand))
        {
            newCommands.Add(newCommand);
        }

        return newCommands;
    }

    protected void ThenEnqueue(IQueueCommand command, Id correlationId, ClaimsPrincipal principal) =>
        TryEnqueue(command.InEnvelope(correlationId, principal));

    protected void ThenEnqueue(IQueueCommand command, Id correlationId) =>
        TryEnqueue(command.InEnvelope(correlationId));

    protected void ThenEnqueue(IQueueCommand command, ClaimsPrincipal principal) =>
        ThenEnqueue(command, RouteCorrelationId, principal);

    protected void ThenEnqueue(IQueueCommand command) =>
        ThenEnqueue(command, RouteCorrelationId);

    protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId, ClaimsPrincipal principal)
    {
        foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
        {
            ThenEnqueue(command, correlationId, principal);
        }
    }

    protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId)
    {
        foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
        {
            ThenEnqueue(command, correlationId);
        }
    }

    protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, ClaimsPrincipal principal)
    {
        foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
        {
            ThenEnqueue(command, principal);
        }
    }

    protected void ThenEnqueue(IEnumerable<IQueueCommand> commands)
    {
        foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
        {
            ThenEnqueue(command);
        }
    }

    protected void ThenEnqueue(Id correlationId, ClaimsPrincipal principal, params IQueueCommand[] commands) =>
        ThenEnqueue(commands.AsEnumerable(), correlationId, principal);

    protected void ThenEnqueue(Id correlationId, params IQueueCommand[] commands) =>
        ThenEnqueue(commands.AsEnumerable(), correlationId);

    protected void ThenEnqueue(ClaimsPrincipal principal, params IQueueCommand[] commands) =>
        ThenEnqueue(commands.AsEnumerable(), principal);

    protected void ThenEnqueue(params IQueueCommand[] commands) =>
        ThenEnqueue(commands.AsEnumerable());

    void TryEnqueue(IQueueCommandEnvelope envelope)
    {
        if(HasErrors)
            throw new InvalidOperationException($"Workflow {this} has one or more errors preventing {envelope.MessageKey.DeclaredType.Name} from enqueueing on {envelope.Info.QueueName}");

        _newCommands.Enqueue(envelope);
    }
}
