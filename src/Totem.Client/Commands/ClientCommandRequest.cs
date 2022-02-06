using Totem.Http;

namespace Totem.Commands;

public class ClientCommandRequest : IMessageRequest
{
    readonly IClientCommandContext<IHttpCommand> _context;
    readonly IClientCommandNegotiator _negotiator;

    public ClientCommandRequest(IClientCommandContext<IHttpCommand> context, IClientCommandNegotiator negotiator)
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
