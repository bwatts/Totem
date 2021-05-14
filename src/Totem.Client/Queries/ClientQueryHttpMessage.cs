using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Queries
{
    public class ClientQueryHttpMessage : ITotemHttpMessage
    {
        readonly IClientQueryContext<IQuery> _context;
        readonly IClientQueryNegotiator _negotiator;

        public ClientQueryHttpMessage(IClientQueryContext<IQuery> context, IClientQueryNegotiator negotiator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
        }

        public async Task SendAsync(HttpClient client, CancellationToken cancellationToken)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            var response = await client.SendAsync(BuildRequest(), cancellationToken);

            _context.Response = await ClientResponse.CreateAsync(response, cancellationToken);
            _context.Result = _negotiator.NegotiateResult(_context.ResultType, _context.Response.ContentType, _context.Response.Content);
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
}