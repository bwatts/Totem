using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Totem.Core;

namespace Totem.Http
{
    public class HttpQueryPipeline : IHttpQueryPipeline
    {
        readonly ILogger _logger;
        readonly IReadOnlyList<IHttpQueryMiddleware> _steps;
        readonly IHttpQueryContextFactory _contextFactory;
        
        public HttpQueryPipeline(
            Id id,
            ILogger<HttpQueryPipeline> logger,
            IReadOnlyList<IHttpQueryMiddleware> steps,
            IHttpQueryContextFactory contextFactory)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _steps = steps ?? throw new ArgumentNullException(nameof(steps));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public Id Id { get; }

        public async Task<IHttpQueryContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
        {
            if(envelope == null)
                throw new ArgumentNullException(nameof(envelope));

            _logger.LogTrace("[query] Run pipeline {@PipelineId} for {@QueryType}.{@QueryId}", Id, envelope.Info.MessageType, envelope.MessageId);

            var context = _contextFactory.Create(Id, envelope);

            await RunStepAsync(0);

            return context;

            async Task RunStepAsync(int index)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("[query] Pipeline {@PipelineId} cancelled", Id);
                    return;
                }

                if(index >= _steps.Count)
                {
                    _logger.LogTrace("[query] Pipeline {@PipelineId} complete", Id);
                    return;
                }

                await _steps[index].InvokeAsync(context!, () => RunStepAsync(index + 1), cancellationToken);
            }
        }
    }
}