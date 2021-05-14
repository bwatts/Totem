using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Events
{
    public class EventPipeline : IEventPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IEventMiddleware> _steps;
        readonly IEventContextFactory _contextFactory;
        
        public EventPipeline(
            Id id,
            ILogger<EventPipeline> logger,
            IReadOnlyList<IEventMiddleware> steps,
            IEventContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IEventContext<IEvent>> RunAsync(IEventEnvelope point, CancellationToken cancellationToken)
        {
            if(point == null)
                throw new ArgumentNullException(nameof(point));

            _logger.LogTrace("[event] Run pipeline {PipelineId} for {EventType}.{EventId} from {TimelineType}.{TimelineId}@{TimelineVersion}", Id, point.MessageType, point.MessageId, point.TimelineType, point.TimelineId, point.TimelineVersion);

            var context = _contextFactory.Create(Id, point);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[event] Pipeline {PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[event] Pipeline {PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}