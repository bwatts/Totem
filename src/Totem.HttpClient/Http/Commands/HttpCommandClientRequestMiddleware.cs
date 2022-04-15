namespace Totem.Http.Commands;

public class HttpCommandClientRequestMiddleware : IHttpCommandClientMiddleware
{
    readonly IMessageClient _client;
    readonly IHttpCommandNegotiator _negotiator;

    public HttpCommandClientRequestMiddleware(IMessageClient client, IHttpCommandNegotiator negotiator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
    }

    public async Task InvokeAsync(IHttpCommandClientContext<IHttpCommand> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        await _client.SendAsync(new HttpCommandClientRequest(context, _negotiator), cancellationToken);

        await next();
    }
}
