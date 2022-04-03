using Microsoft.AspNetCore.SignalR.Client;

namespace Totem.Subscriptions;

public class SubscriptionClient : ISubscriptionClient
{
    readonly HubConnection _connection;

    public SubscriptionClient(HubConnection connection) =>
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    public Task<Id> StartHubAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task SubscribeToQueryAsync(string etag, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task UnsubscribeFromQueryAsync(string etag, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
