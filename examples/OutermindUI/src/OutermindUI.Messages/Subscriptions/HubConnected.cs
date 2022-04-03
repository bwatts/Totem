namespace OutermindUI.Subscriptions;

public class HubConnected : IEvent
{
    public HubConnected(Id connectionId) =>
        ConnectionId = connectionId ?? throw new ArgumentNullException(nameof(connectionId));

    public Id ConnectionId { get; }
}
