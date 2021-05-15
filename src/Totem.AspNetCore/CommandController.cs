using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Totem.Commands;
using Totem.Core;

namespace Totem
{
    public class CommandController<TCommand> : RequestController
        where TCommand : ICommand
    {
        readonly ILogger _logger;
        readonly ICommandPipeline _pipeline;
        readonly ICorrelationIdAccessor _correlationIdAccessor;

        public CommandController(ILogger<CommandController<TCommand>> logger, ICommandPipeline pipeline, ICorrelationIdAccessor correlationIdAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));
        }

        [ErrorInfoActionFilter]
        public async Task<IActionResult> Handle([FromTotem] TCommand command, CancellationToken cancellationToken)
        {
            var commandId = Id.NewId();
            var correlationId = _correlationIdAccessor.CorrelationId ?? Id.NewId();
            var envelope = new CommandEnvelope(command, commandId, correlationId, User);

            _logger.LogTrace("[command] {RequestMethod} {@CommandType}.{@CommandId}", Request.Method, envelope.MessageType, envelope.MessageId);

            var context = await _pipeline.RunAsync(envelope, cancellationToken);

            if(context.HasErrors)
            {
                _logger.LogError("[command] {RequestMethod} {@CommandType}.{@CommandId} failed: {Errors}", Request.Method, envelope.MessageType, envelope.MessageId, context.Errors);

                return new ErrorInfoResult(context.Errors);
            }

            return Respond(context.ResponseCode, context.ResponseHeaders, context.ResponseContent);
        }
    }
}