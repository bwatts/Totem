namespace Totem.Subscriptions;

public interface ISubscriptionClient
{
    Task<Id> StartHubAsync(CancellationToken cancellationToken);
    Task SubscribeToQueryAsync(string etag, CancellationToken cancellationToken);
    Task UnsubscribeFromQueryAsync(string etag, CancellationToken cancellationToken);
}
