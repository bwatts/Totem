using System.Security.Claims;
using Totem.Http;

namespace Totem;

public static class HttpCommandClientExtensions
{
    public static Task SendAsync(this IHttpCommandClient client, IHttpCommand command, Id correlationId, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId, principal), cancellationToken);

    public static Task SendAsync(this IHttpCommandClient client, IHttpCommand command, Id correlationId, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(correlationId), cancellationToken);

    public static Task SendAsync(this IHttpCommandClient client, IHttpCommand command, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(principal), cancellationToken);

    public static Task SendAsync(this IHttpCommandClient client, IHttpCommand command, CancellationToken cancellationToken) =>
        client.SendAndExpectNoErrorsAsync(command.InEnvelope(), cancellationToken);

    static async Task SendAndExpectNoErrorsAsync(this IHttpCommandClient client, IHttpCommandEnvelope envelope, CancellationToken cancellationToken)
    {
        if(client is null)
            throw new ArgumentNullException(nameof(client));

        var context = await client.SendAsync(envelope, cancellationToken);

        context.Errors.ExpectNone();
    }
}
