namespace Totem.Http;

public interface IMessageClient
{
    Task SendAsync(IMessageRequest request, CancellationToken cancellationToken);
}
