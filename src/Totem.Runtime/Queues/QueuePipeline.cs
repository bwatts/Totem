using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Queues
{
    public class QueuePipeline : IQueuePipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IQueueMiddleware> _steps;
        readonly IQueueContextFactory _contextFactory;
        
        public QueuePipeline(
            Id id,
            ILogger<QueuePipeline> logger,
            IReadOnlyList<IQueueMiddleware> steps,
            IQueueContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IQueueContext<IQueueCommand>> RunAsync(IQueueEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[queue {QueueName}] Run pipeline {@PipelineId} for {@CommandType}.{@CommandId}", envelope.QueueName, Id, envelope.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[queue {QueueName}] Pipeline {@PipelineId} cancelled", envelope.QueueName, Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[queue {QueueName}] Pipeline {@PipelineId} complete", envelope.QueueName, Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}