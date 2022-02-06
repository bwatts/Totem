using System.Security.Claims;
using Totem.Http;

namespace Totem;

public static class TotemHttpClientExtensions
{
    public static Task SendAsync(this ITotemHttpClient client, IHttpCommand command, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId, principal), cancellationToken);

    public static Task SendAsync(this ITotemHttpClient client, IHttpCommand command, Id correlationId, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId), cancellationToken);

    public static Task SendAsync(this ITotemHttpClient client, IHttpCommand command, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(principal), cancellationToken);

    public static Task SendAsync(this ITotemHttpClient client, IHttpCommand command, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(), cancellationToken);

    public static Task<object?> SendAsync(this ITotemHttpClient client, IHttpQuery query, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(query.InEnvelope(correlationId, principal), cancellationToken);

    public static Task<object?> SendAsync(this ITotemHttpClient client, IHttpQuery query, Id correlationId, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(query.InEnvelope(correlationId), cancellationToken);

    public static Task<object?> SendAsync(this ITotemHttpClient client, IHttpQuery query, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(query.InEnvelope(principal), cancellationToken);

    public static Task<object?> SendAsync(this ITotemHttpClient client, IHttpQuery query, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(query.InEnvelope(), cancellationToken);

    public static Task<TResult> SendAsync<TResult>(this ITotemHttpClient client, IHttpQuery<TResult> query, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId, principal), cancellationToken);

    public static Task<TResult> SendAsync<TResult>(this ITotemHttpClient client, IHttpQuery<TResult> query, Id correlationId, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(correlationId), cancellationToken);

    public static Task<TResult> SendAsync<TResult>(this ITotemHttpClient client, IHttpQuery<TResult> query, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(principal), cancellationToken);

    public static Task<TResult> SendAsync<TResult>(this ITotemHttpClient client, IHttpQuery<TResult> query, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync<TResult>(query.InEnvelope(), cancellationToken);

    static async Task SendAndExpectNoErrorsAsync(this ITotemHttpClient client, IHttpCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        var context = await client.SendAsync(envelope, cancellationToken);

        context.Errors.ExpectNone();
    }

    static async Task<object?> SendAndExpectNoErrorsAsync(this ITotemHttpClient client, IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        var context = await client.SendAsync(envelope, cancellationToken);
        var result = context.Result;
        var resultType = envelope.Info.Result.DeclaredType;

        context.Errors.ExpectNone();

        if(result is null || !resultType.IsAssignableFrom(result.GetType()))
        {
            throw new InvalidOperationException($"Expected result of type {resultType} but found {result?.GetType().ToString() ?? "null"}");
        }

        return result;
    }

    static async Task<TResult> SendAndExpectNoErrorsAsync<TResult>(this ITotemHttpClient client, IHttpQueryEnvelope envelope, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        var context = await client.SendAsync(envelope, cancellationToken);

        context.Errors.ExpectNone();

        if(context.Result is not TResult result)
            throw new InvalidOperationException($"Expected result of type {typeof(TResult)} but found {context.Result?.GetType().ToString() ?? "null"}");

        return result;
    }
}
