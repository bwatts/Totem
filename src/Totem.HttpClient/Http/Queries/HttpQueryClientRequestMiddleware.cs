namespace Totem.Http.Queries;

public class HttpQueryClientRequestMiddleware : IHttpQueryClientMiddleware
{
    readonly IMessageClient _client;
    readonly IHttpQueryNegotiator _negotiator;

    public HttpQueryClientRequestMiddleware(IMessageClient client, IHttpQueryNegotiator negotiator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
    }

    public async Task InvokeAsync(IHttpQueryClientContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken)
    {
        if(context is null)
            throw new ArgumentNullException(nameof(context));

        if(next is null)
            throw new ArgumentNullException(nameof(next));

        await _client.SendAsync(new HttpQueryClientRequest(context, _negotiator), cancellationToken);

        await next();
    }
}
