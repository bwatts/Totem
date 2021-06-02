using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem.Queries
{
    public class ClientQueryRequestMiddleware : IClientQueryMiddleware
    {
        readonly IMessageClient _client;
        readonly IClientQueryNegotiator _negotiator;

        public ClientQueryRequestMiddleware(IMessageClient client, IClientQueryNegotiator negotiator)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _negotiator = negotiator ?? throw new ArgumentNullException(nameof(negotiator));
        }

        public async Task InvokeAsync(IClientQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            await _client.SendAsync(new ClientQueryRequest(context, _negotiator), cancellationToken);

            await next();
        }
    }
}