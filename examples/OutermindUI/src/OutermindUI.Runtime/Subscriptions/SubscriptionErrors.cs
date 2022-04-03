namespace OutermindUI.Subscriptions;

public static class SubscriptionErrors
{
    public static readonly ErrorInfo HubAlreadyConnected = new(nameof(HubAlreadyConnected), ErrorLevel.Conflict);
    public static readonly ErrorInfo HubAlreadyConnecting = new(nameof(HubAlreadyConnecting), ErrorLevel.Conflict);
    public static readonly ErrorInfo HubNotConnecting = new(nameof(HubNotConnecting), ErrorLevel.Conflict);
}
