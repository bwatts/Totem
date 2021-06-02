using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Local
{
    public interface ILocalQueryMiddleware
    {
        Task InvokeAsync(ILocalQueryContext<ILocalQuery> context, Func<Task> next, CancellationToken cancellationToken);
    }
}