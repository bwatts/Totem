using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Queries
{
    public class QueryPipeline : IQueryPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IQueryMiddleware> _steps;
        readonly IQueryContextFactory _contextFactory;
        
        public QueryPipeline(
            Id id,
            ILogger<QueryPipeline> logger,
            IReadOnlyList<IQueryMiddleware> steps,
            IQueryContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IQueryContext<IQuery>> RunAsync(IQueryEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[query] Run pipeline {PipelineId} for {QueryType}.{QueryId}", Id, envelope.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[query] Pipeline {PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[query] Pipeline {PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}