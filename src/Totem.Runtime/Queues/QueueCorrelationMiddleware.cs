using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Queues
{
    public class QueueCorrelationMiddleware : IQueueMiddleware
    {
        readonly ICorrelationIdAccessor _correlationIdAccessor;

        public QueueCorrelationMiddleware(ICorrelationIdAccessor correlationIdAccessor) =>
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));

        public async Task InvokeAsync(IQueueContext<IQueueCommand> context, Func<Task> next, CancellationToken cancellationToken)
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