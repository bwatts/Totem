using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Commands
{
    public class CommandPipeline : ICommandPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<ICommandMiddleware> _steps;
        readonly ICommandContextFactory _contextFactory;

        public CommandPipeline(
            Id id,
            ILogger<CommandPipeline> logger,
            IReadOnlyList<ICommandMiddleware> steps,
            ICommandContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<ICommandContext<ICommand>> RunAsync(ICommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[command] Run pipeline {PipelineId} for {CommandType}.{CommandId}", Id, envelope.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[command] Pipeline {PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[command] Pipeline {PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}