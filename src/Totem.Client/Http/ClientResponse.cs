using System.Net;
using System.Net.Http.Headers;

namespace Totem.Http;

public class ClientResponse
{
    ClientResponse(HttpResponseMessage message, string content)
    {
        StatusCode = message.StatusCode;
        StatusCode = message.StatusCode;
        IsSuccessStatus = message.IsSuccessStatusCode;
        ReasonPhrase = message.ReasonPhrase;
        Headers = message.Headers;
        ContentType = message.Content.Headers.ContentType?.MediaType?.ToString() ?? ContentTypes.PlainText;
        Content = content;
    }

    public HttpStatusCode? StatusCode { get; }
    public bool IsSuccessStatus { get; }
    public string? ReasonPhrase { get; }
    public HttpResponseHeaders Headers { get; }
    public string ContentType { get; }
    public string Content { get; }

    public override string ToString() => $"{StatusCode} {ReasonPhrase}";

    public static async Task<ClientResponse> CreateAsync(HttpResponseMessage message, CancellationToken cancellationToken)
    {
        if(message is null)
            throw new ArgumentNullException(nameof(message));

        return new ClientResponse(message, await message.Content.ReadAsStringAsync(cancellationToken));
    }
}
