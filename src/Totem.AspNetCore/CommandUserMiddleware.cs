using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Totem.Commands;

namespace Totem
{
    public class CommandUserMiddleware : ICommandMiddleware
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public CommandUserMiddleware(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        public async Task InvokeAsync(ICommandContext<ICommand> context, Func<Task> next, CancellationToken cancellationToken)
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