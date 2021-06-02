using System;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Http
{
    public interface IHttpQueryMiddleware
    {
        Task InvokeAsync(IHttpQueryContext<IHttpQuery> context, Func<Task> next, CancellationToken cancellationToken);
    }
}