using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Totem.Queries;
using Totem.Core;

namespace Totem
{
    public class QueryController<TQuery> : RequestController
        where TQuery : IQuery
    {
        readonly ILogger _logger;
        readonly IQueryPipeline _pipeline;
        readonly ICorrelationIdAccessor _correlationIdAccessor;

        public QueryController(ILogger<QueryController<TQuery>> logger, IQueryPipeline pipeline, ICorrelationIdAccessor correlationIdAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
        }

        [ErrorInfoActionFilter]
        public async Task<IActionResult> Handle([FromTotem] TQuery query, CancellationToken cancellationToken)
        {
            var queryId = Id.NewId();
            var correlationId = _correlationIdAccessor.CorrelationId ?? Id.NewId();
            var envelope = new QueryEnvelope(query, queryId, correlationId, User);

            _logger.LogTrace("[query] {RequestMethod} {QueryType}.{QueryId}", Request.Method, envelope.MessageType, envelope.MessageId);

            var context = await _pipeline.RunAsync(envelope, cancellationToken);

            if(context.HasErrors)
            {
                _logger.LogError("[query] {RequestMethod} {QueryType}.{QueryId} failed: {Errors}", Request.Method, envelope.MessageType, envelope.MessageId, context.Errors);

                return new ErrorInfoResult(context.Errors);
            }

            return Respond(context.ResponseCode, context.ResponseHeaders, context.ResponseContent);
        }
    }
}