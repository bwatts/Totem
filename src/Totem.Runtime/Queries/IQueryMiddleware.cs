using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Queries
{
    public interface IQueryMiddleware
    {
        Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken);
    }
}