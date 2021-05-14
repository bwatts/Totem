using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Totem.Core;
using Totem.Events;
using Totem.Workflows;

namespace Totem
{
    public abstract class Workflow : EventSourced, IWorkflow
    {
        public static readonly Id RouteCorrelationId = Id.NewId();

        readonly ConcurrentQueue<IQueueEnvelope> _newCommands = new();

        public bool HasNewCommands => !_newCommands.IsEmpty;
        public IEnumerable<IQueueEnvelope> NewCommands => _newCommands.Select(x => x);

        protected void ThenEnqueue(IQueueCommand command, Id correlationId, ClaimsPrincipal principal, Text queueName) =>
            TryEnqueue(command.InEnvelope(correlationId, principal, queueName));

        protected void ThenEnqueue(IQueueCommand command, Id correlationId, ClaimsPrincipal principal) =>
            TryEnqueue(command.InEnvelope(correlationId, principal));

        protected void ThenEnqueue(IQueueCommand command, Id correlationId, Text queueName) =>
            TryEnqueue(command.InEnvelope(correlationId, queueName));

        protected void ThenEnqueue(IQueueCommand command, Id correlationId) =>
            TryEnqueue(command.InEnvelope(correlationId));

        protected void ThenEnqueue(IQueueCommand command, ClaimsPrincipal principal, Text queueName) =>
            ThenEnqueue(command, RouteCorrelationId, principal, queueName);

        protected void ThenEnqueue(IQueueCommand command, ClaimsPrincipal principal) =>
            ThenEnqueue(command, RouteCorrelationId, principal);

        protected void ThenEnqueue(IQueueCommand command, Text queueName) =>
            ThenEnqueue(command, RouteCorrelationId, queueName);

        protected void ThenEnqueue(IQueueCommand command) =>
            ThenEnqueue(command, RouteCorrelationId);

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId, ClaimsPrincipal principal, Text queueName)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, correlationId, principal, queueName);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId, ClaimsPrincipal principal)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, correlationId, principal);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId, Text queueName)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, correlationId, queueName);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Id correlationId)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, correlationId);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, ClaimsPrincipal principal, Text queueName)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, principal, queueName);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, ClaimsPrincipal principal)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, principal);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands, Text queueName)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command, queueName);
            }
        }

        protected void ThenEnqueue(IEnumerable<IQueueCommand> commands)
        {
            foreach(var command in commands ?? throw new ArgumentNullException(nameof(commands)))
            {
                ThenEnqueue(command);
            }
        }

        protected void ThenEnqueue(Id correlationId, ClaimsPrincipal principal, Text queueName, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), correlationId, principal, queueName);

        protected void ThenEnqueue(Id correlationId, ClaimsPrincipal principal, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), correlationId, principal);

        protected void ThenEnqueue(Id correlationId, Text queueName, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), correlationId, queueName);

        protected void ThenEnqueue(Id correlationId, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), correlationId);

        protected void ThenEnqueue(ClaimsPrincipal principal, Text queueName, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), principal, queueName);

        protected void ThenEnqueue(ClaimsPrincipal principal, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), principal);

        protected void ThenEnqueue(Text queueName, params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable(), queueName);

        protected void ThenEnqueue(params IQueueCommand[] commands) =>
            ThenEnqueue(commands.AsEnumerable());

        void TryEnqueue(IQueueEnvelope envelope)
        {
            if(HasErrors)
                throw new InvalidOperationException($"Workflow {this} has one or more errors preventing {envelope.MessageType.Name} from enqueueing on {envelope.QueueName}");

            _newCommands.Enqueue(envelope);
        }
    }
}