namespace Totem.Http.Commands;

public class HttpCommandClientRequest : IMessageRequest
{
    readonly IHttpCommandClientContext<IHttpCommand> _context;
    readonly IHttpCommandNegotiator _negotiator;

    public HttpCommandClientRequest(IHttpCommandClientContext<IHttpCommand> context, IHttpCommandNegotiator negotiator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
    }

    public async Task SendAsync(HttpClient client, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        var response = await client.SendAsync(BuildRequest(), cancellationToken);

        _context.Response = await ClientResponse.CreateAsync(response, cancellationToken);
    }

    HttpRequestMessage BuildRequest()
    {
        var request = _negotiator.Negotiate(_context.Command, _context.ContentType);

        foreach(var (name, values) in _context.Headers)
        {
            request.Headers.Add(name, values);
        }

        return request;
    }
}
