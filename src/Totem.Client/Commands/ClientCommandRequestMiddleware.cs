using Totem.Http;

namespace Totem.Commands;

public class ClientCommandRequestMiddleware : IClientCommandMiddleware
{
    readonly IMessageClient _client;
    readonly IClientCommandNegotiator _negotiator;

    public ClientCommandRequestMiddleware(IMessageClient client, IClientCommandNegotiator negotiator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
    }

    public async Task InvokeAsync(IClientCommandContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        await _client.SendAsync(new ClientCommandRequest(context, _negotiator), cancellationToken);

        await next();
    }
}
