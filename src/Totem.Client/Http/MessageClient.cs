namespace Totem.Http;

public class MessageClient : IMessageClient
{
    readonly HttpClient _httpClient;

    public MessageClient(HttpClient httpClient) =>
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task SendAsync(IMessageRequest message, CancellationToken cancellationToken)
    {
        if(message is null)
            throw new ArgumentNullException(nameof(message));

        await message.SendAsync(_httpClient, cancellationToken);
    }
}
