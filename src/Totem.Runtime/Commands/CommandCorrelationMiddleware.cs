using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Commands
{
    public class CommandCorrelationMiddleware : ICommandMiddleware
    {
        readonly ICorrelationIdAccessor _correlationIdAccessor;

        public CommandCorrelationMiddleware(ICorrelationIdAccessor correlationIdAccessor) =>
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));

        public async Task InvokeAsync(ICommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            if(context.CorrelationId == null)
            {
                context.CorrelationId = _correlationIdAccessor.CorrelationId ?? Id.NewId();
            }

            await next();
        }
    }
}