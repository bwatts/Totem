using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Totem.Queries;

namespace Totem
{
    public class QueryUserMiddleware : IQueryMiddleware
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public QueryUserMiddleware(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public async Task InvokeAsync(IQueryContext<IQuery> context, Func<Task> next, CancellationToken cancellationToken)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            if(next == null)
                throw new ArgumentNullException(nameof(next));

            context.User = _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

            await next();
        }
    }
}