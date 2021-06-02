using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Totem.Http;

namespace Totem
{
    public static class TotemClientExtensions
    {
        public static Task SendAsync(this ITotemClient client, IHttpCommand command, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId, principal), cancellationToken);

        public static Task SendAsync(this ITotemClient client, IHttpCommand command, Id correlationId, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId), cancellationToken);

        public static Task SendAsync(this ITotemClient client, IHttpCommand command, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(principal), cancellationToken);

        public static Task SendAsync(this ITotemClient client, IHttpCommand command, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(command.InEnvelope(), cancellationToken);

        public static Task<object?> SendAsync(this ITotemClient client, IHttpQuery query, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(query.InEnvelope(correlationId, principal), cancellationToken);

        public static Task<object?> SendAsync(this ITotemClient client, IHttpQuery query, Id correlationId, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(query.InEnvelope(correlationId), cancellationToken);

        public static Task<object?> SendAsync(this ITotemClient client, IHttpQuery query, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(query.InEnvelope(principal), cancellationToken);

        public static Task<object?> SendAsync(this ITotemClient client, IHttpQuery query, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync(query.InEnvelope(), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IHttpQuery<TResult> query, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId, principal), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IHttpQuery<TResult> query, Id correlationId, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IHttpQuery<TResult> query, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(principal), cancellationToken);

        public static Task<TResult> SendAsync<TResult>(this ITotemClient client, IHttpQuery<TResult> query, CancellationToken cancellationToken) =>
            client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(), cancellationToken);

        static async Task SendAndExpectNoErrorsAsync(this ITotemClient client, IHttpCommandEnvelope envelope, CancellationToken cancellationToken)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            var context = await client.SendAsync(envelope, cancellationToken);

            context.Errors.ExpectNone();
        }

        static async Task<object?> SendAndExpectNoErrorsAsync(this ITotemClient client, IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
        {
            if(client == null)
                throw new ArgumentNullException(nameof(client));

            var context = await client.SendAsync(envelope, cancellationToken);
            var result = context.Result;
            var resultType = envelope.Info.ResultType;

            context.Errors.ExpectNone();

            if(result == null || !resultType.IsAssignableFrom(result.GetType()))
            {
                throw new InvalidOperationException($"Expected result of type {resultType} but found {result?.GetType().ToString() ?? "null"}");
            }

            return result;
        }

        static async Task<TResult> SendAndExpectNoErrorsAsync<TResult>(this ITotemClient client, IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
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