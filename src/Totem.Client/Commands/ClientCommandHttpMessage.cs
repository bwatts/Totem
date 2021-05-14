using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Commands
{
    public class ClientCommandHttpMessage : ITotemHttpMessage
    {
        readonly IClientCommandContext<ICommand> _context;
        readonly IClientCommandNegotiator _negotiator;

        public ClientCommandHttpMessage(IClientCommandContext<ICommand> context, IClientCommandNegotiator negotiator)
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
}