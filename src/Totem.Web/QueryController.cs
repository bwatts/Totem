using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Totem.Http;

namespace Totem
{
    public class QueryController<TQuery> : RequestController
        where TQuery : IHttpQuery
    {
        readonly ILogger _logger;
        readonly IHttpQueryPipeline _pipeline;

        public QueryController(ILogger<QueryController<TQuery>> logger, IHttpQueryPipeline pipeline)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        [ErrorInfoActionFilter]
        public async Task<IActionResult> Handle([FromTotem] TQuery query, CancellationToken cancellationToken)
        {
            var queryId = Id.NewId();
            var correlationId = Id.NewId();
            var envelope = new HttpQueryEnvelope(queryId, query, HttpQueryInfo.From(query), correlationId, User);

            _logger.LogTrace("[query] {RequestMethod} {@QueryType}.{@QueryId}", Request.Method, envelope.Info.MessageType, envelope.MessageId);

            var context = await _pipeline.RunAsync(envelope, cancellationToken);

            if(context.HasErrors)
            {
                _logger.LogError("[query] {RequestMethod} {@QueryType}.{@QueryId} failed: {Errors}", Request.Method, envelope.Info.MessageType, envelope.MessageId, context.Errors);

                return new ErrorInfoResult(context.Errors);
            }

            return Respond(context.ResponseCode, context.ResponseHeaders, context.ResponseContent);
        }
    }
}