namespace Totem.Http;

public interface IMessageRequest
{
    Task SendAsync(HttpClient client, CancellationToken cancellationToken);
}
