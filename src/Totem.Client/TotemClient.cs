using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Commands;
using Totem.Core;
using Totem.Queries;

namespace Totem
{
    public class TotemClient : ITotemClient
    {
        readonly IClientCommandPipeline _commandPipeline;
        readonly IClientQueryPipeline _queryPipeline;

        public TotemClient(IClientCommandPipeline commandPipeline, IClientQueryPipeline queryPipeline)
        {
            _commandPipeline = commandPipeline ?? throw new ArgumentNullException(nameof(commandPipeline));
            _queryPipeline = queryPipeline ?? throw new ArgumentNullException(nameof(queryPipeline));
        }

        public Task<IClientCommandContext<ICommand>> SendAsync(ICommandEnvelope envelope, CancellationToken cancellationToken) =>
            _commandPipeline.RunAsync(envelope, cancellationToken);

        public Task<IClientQueryContext<IQuery>> SendAsync(IQueryEnvelope envelope, CancellationToken cancellationToken) =>
            _queryPipeline.RunAsync(envelope, cancellationToken);
    }
}