using Microsoft.AspNetCore.Mvc;
using Totem.Core;
using Totem.Http;

namespace Totem;

public class HttpCommandController<TCommand> : HttpRequestController
    where TCommand : IHttpCommand
{
    readonly ILogger _logger;
    readonly IHttpCommandPipeline _pipeline;

    public HttpCommandController(ILogger<HttpCommandController<TCommand>> logger, IHttpCommandPipeline pipeline)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
    }

    [ErrorInfoActionFilter]
    public async Task<IActionResult> Handle([FromTotem] TCommand command, CancellationToken cancellationToken)
    {
        var commandId = new ItemKey(typeof(TCommand), Id.NewId());
        var correlationId = Id.NewId();
        var envelope = new HttpCommandEnvelope(commandId, command, HttpCommandInfo.From(typeof(TCommand)), correlationId, User);

        _logger.LogTrace("[command] {RequestMethod} {@CommandType}.{@CommandId}", Request.Method, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id);

        var context = await _pipeline.RunAsync(envelope, cancellationToken);

        if(context.HasErrors)
        {
            _logger.LogError("[command] {RequestMethod} {@CommandType}.{@CommandId} failed: {Errors}", Request.Method, envelope.MessageKey.DeclaredType, envelope.MessageKey.Id, context.Errors);

            return new ErrorInfoResult(context.Errors);
        }

        return Respond(context.ResponseCode, context.ResponseHeaders, context.ResponseContent);
    }
}
