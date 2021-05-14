using System;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Queries
{
    public class QueryCorrelationMiddleware : IQueryMiddleware
    {
        readonly ICorrelationIdAccessor _correlationIdAccessor;

        public QueryCorrelationMiddleware(ICorrelationIdAccessor correlationIdAccessor) =>
            _correlationIdAccessor = correlationIdAccessor ?? throw new ArgumentNullException(nameof(correlationIdAccessor));

        public async Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken)
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