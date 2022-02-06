using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Queues;

public static class QueueCommandClientExtensions
{
    public static Task EnqueueAsync(this IQueueClient client, IQueueCommand command, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        return client.EnqueueAsync(command.InEnvelope(correlationId, principal), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IQueueCommand command, Id correlationId, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        return client.EnqueueAsync(command.InEnvelope(correlationId), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IQueueCommand command, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        return client.EnqueueAsync(command.InEnvelope(principal), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IQueueCommand command, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        return client.EnqueueAsync(command.InEnvelope(), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IEnumerable<IQueueCommand> commands, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        if(commands is null)
            throw new ArgumentNullException(nameof(commands));

        return client.EnqueueAsync(commands.Select(x => x.InEnvelope(correlationId, principal)), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IEnumerable<IQueueCommand> commands, Id correlationId, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        if(commands is null)
            throw new ArgumentNullException(nameof(commands));

        return client.EnqueueAsync(commands.Select(x => x.InEnvelope(correlationId)), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IEnumerable<IQueueCommand> commands, ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        if(commands is null)
            throw new ArgumentNullException(nameof(commands));

        return client.EnqueueAsync(commands.Select(x => x.InEnvelope(principal)), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, IEnumerable<IQueueCommand> commands, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        if(commands is null)
            throw new ArgumentNullException(nameof(commands));

        return client.EnqueueAsync(commands.Select(x => x.InEnvelope()), cancellationToken);
    }

    public static Task EnqueueAsync(this IQueueClient client, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken, params IQueueCommand[] commands) =>
        client.EnqueueAsync(commands.AsEnumerable(), correlationId, principal, cancellationToken);

    public static Task EnqueueAsync(this IQueueClient client, Id correlationId, CancellationToken cancellationToken, params IQueueCommand[] commands) =>
        client.EnqueueAsync(commands.AsEnumerable(), correlationId, cancellationToken);

    public static Task EnqueueAsync(this IQueueClient client, ClaimsPrincipal principal, CancellationToken cancellationToken, params IQueueCommand[] commands) =>
        client.EnqueueAsync(commands.AsEnumerable(), principal, cancellationToken);
}
