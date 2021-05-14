using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Commands
{
    public class ClientCommandHttpMiddleware : IClientCommandMiddleware
    {
        readonly ITotemHttpClient _client;
        readonly IClientCommandNegotiator _negotiator;

        public ClientCommandHttpMiddleware(ITotemHttpClient client, IClientCommandNegotiator negotiator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
        }

        public async Task InvokeAsync(IClientCommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await _client.SendAsync(new ClientCommandHttpMessage(context, _negotiator), cancellationToken);

            await next();
        }
    }
}