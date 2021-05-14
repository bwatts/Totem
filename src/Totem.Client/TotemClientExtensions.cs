using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem
{
    public static class TotemClientExtensions
    {
        public static Task SendAsync(this ITotemClient client, ICommand command, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId, principal), cancellationToken);

        public static Task SendAsync(this ITotemClient client, ICommand command, Id correlationId, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId), cancellationToken);

        public static Task SendAsync(this ITotemClient client, ICommand command, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(principal), cancellationToken);

        public static Task SendAsync(this ITotemClient client, ICommand command, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IQuery<TResult> query, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId, principal), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IQuery<TResult> query, Id correlationId, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IQuery<TResult> query, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(principal), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IQuery<TResult> query, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(), cancellationToken);

        static async Task SendAndExpectNoErrorsAsync(this ITotemClient client, ICommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            var context = await client.SendAsync(envelope, cancellationToken);

            context.Errors.ExpectNone();
        }

        static async Task<TResult> SendAndExpectNoErrorsAsync<TResult>(this ITotemClient client, IQueryEnvelope envelope, CancellationToken cancellationToken)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            var context = await client.SendAsync(envelope, cancellationToken);

            context.Errors.ExpectNone();

            if(context.Result is not TResult result)
                throw new InvalidOperationException($"Expected result of type {typeof(TResult)} but found {context.Result?.GetType().ToString() ?? "null"}");

            return result;
        }
    }
}