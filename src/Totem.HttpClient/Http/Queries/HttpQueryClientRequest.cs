namespace Totem.Http.Queries;

public class HttpQueryClientRequest : IMessageRequest
{
    readonly IHttpQueryClientContext<IHttpQuery> _context;
    readonly IHttpQueryNegotiator _negotiator;

    public HttpQueryClientRequest(IHttpQueryClientContext<IHttpQuery> context, IHttpQueryNegotiator negotiator)
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

        _negotiator.NegotiateResult(_context);
    }

    HttpRequestMessage BuildRequest()
    {
        var request = _negotiator.Negotiate(_context.Query);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(_context.Accept));

        foreach(var (name, values) in _context.Headers)
        {
            request.Headers.Add(name, values);
        }

        return request;
    }
}
