using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Totem.Local
{
    public class LocalCommandPipeline : ILocalCommandPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<ILocalCommandMiddleware> _steps;
        readonly ILocalCommandContextFactory _contextFactory;

        public LocalCommandPipeline(
            Id id,
            ILogger<LocalCommandPipeline> logger,
            IReadOnlyList<ILocalCommandMiddleware> steps,
            ILocalCommandContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<ILocalCommandContext<ILocalCommand>> RunAsync(ILocalCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[command] Run pipeline {@PipelineId} for {@CommandType}.{@CommandId}", Id, envelope.Info.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[command] Pipeline {@PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[command] Pipeline {@PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}