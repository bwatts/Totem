using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Queries
{
    public class ClientQueryHttpMiddleware : IClientQueryMiddleware
    {
        readonly ITotemHttpClient _client;
        readonly IClientQueryNegotiator _negotiator;

        public ClientQueryHttpMiddleware(ITotemHttpClient client, IClientQueryNegotiator negotiator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
        }

        public async Task InvokeAsync(IClientQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await _client.SendAsync(new ClientQueryHttpMessage(context, _negotiator), cancellationToken);

            await next();
        }
    }
}