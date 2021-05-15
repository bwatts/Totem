using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;
using Totem.Routes;

namespace Totem.Workflows
{
    public class WorkflowPipeline : IWorkflowPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IRouteMiddleware> _steps;
        readonly IRouteContextFactory _contextFactory;
        
        public WorkflowPipeline(
            Id id,
            ILogger<WorkflowPipeline> logger,
            IReadOnlyList<IRouteMiddleware> steps,
            IRouteContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IRouteContext<IEvent>> RunAsync(IEventEnvelope envelope, IRouteAddress address, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            if(address == null)
                throw new ArgumentNullException(nameof(address));

            _logger.LogTrace("[workflow] Run pipeline {@PipelineId} for {@RouteType}.{@RouteId} and event {@EventType}.{@EventId} from {@TimelineType}.{@TimelineId}@{TimelineVersion}", Id, address.RouteType, address.RouteId, envelope.MessageType, envelope.MessageId, envelope.TimelineType, envelope.TimelineId, envelope.TimelineVersion);

            var context = _contextFactory.Create(Id, envelope, address);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[workflow] Pipeline {@PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[workflow] Pipeline {@PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}