using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Totem.Http;

namespace Totem
{
    public class CommandController<TCommand> : RequestController
        where TCommand : IHttpCommand
    {
        readonly ILogger _logger;
        readonly IHttpCommandPipeline _pipeline;

        public CommandController(ILogger<CommandController<TCommand>> logger, IHttpCommandPipeline pipeline)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        [ErrorInfoActionFilter]
        public async Task<IActionResult> Handle([FromTotem] TCommand command, CancellationToken cancellationToken)
        {
            var commandId = Id.NewId();
            var correlationId = Id.NewId();
            var envelope = new HttpCommandEnvelope(commandId, command, HttpCommandInfo.From(command), correlationId, User);

            _logger.LogTrace("[command] {RequestMethod} {@CommandType}.{@CommandId}", Request.Method, envelope.Info.MessageType, envelope.MessageId);

            var context = await _pipeline.RunAsync(envelope, cancellationToken);

            if(context.HasErrors)
            {
                _logger.LogError("[command] {RequestMethod} {@CommandType}.{@CommandId} failed: {Errors}", Request.Method, envelope.Info.MessageType, envelope.MessageId, context.Errors);

                return new ErrorInfoResult(context.Errors);
            }

            return Respond(context.ResponseCode, context.ResponseHeaders, context.ResponseContent);
        }
    }
}