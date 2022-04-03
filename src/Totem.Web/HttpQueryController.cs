using Microsoft.AspNetCore.Mvc;
using Totem.Core;
using Totem.Http;

namespace Totem;

public class HttpQueryController<TQuery> : HttpRequestController
    where TQuery : IHttpQuery
{
    readonly ILogger _logger;
    readonly IHttpQueryPipeline _pipeline;

    public HttpQueryController(ILogger<HttpQueryController<TQuery>> logger, IHttpQueryPipeline pipeline)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    [ErrorInfoActionFilter]
    public async Task<IActionResult> Handle([FromTotem] TQuery query, CancellationToken cancellationToken)
    {
        var queryId = Id.NewId();
        var correlationId = Id.NewId();
        var envelope = new HttpQueryEnvelope(new ItemKey(typeof(TQuery), queryId), query, HttpQueryInfo.From(query), correlationId, User);

        _logger.LogTrace("[query] {RequestMethod} {@QueryType}.{@QueryId}", Request.Method, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = await _pipeline.RunAsync(envelope, cancellationToken);

        if(context.HasErrors)
        {
            _logger.LogError("[query] {RequestMethod} {@QueryType}.{@QueryId} failed: {Errors}", Request.Method, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id, context.Errors);

            return new ErrorInfoResult(context.Errors);
        }

        return Respond(context.ResponseCode, context.ResponseHeaders, context.Result);
    }
}
