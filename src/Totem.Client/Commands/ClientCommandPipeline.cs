using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Commands
{
    public class ClientCommandPipeline : IClientCommandPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IClientCommandMiddleware> _steps;
        readonly IClientCommandContextFactory _contextFactory;
        
        public ClientCommandPipeline(
            Id id,
            ILogger<ClientCommandPipeline> logger,
            IReadOnlyList<IClientCommandMiddleware> steps,
            IClientCommandContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IClientCommandContext<ICommand>> RunAsync(ICommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[command] Run client pipeline {@PipelineId} for {@CommandType}.{@CommandId}", Id, envelope.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[command] Client pipeline {@PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[command] Client pipeline {@PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}