using Totem.Subscriptions;

namespace OutermindUI.Subscriptions.Topics;

public class HubTopic : Topic
{
    public static readonly Id SingleInstanceId = (Id) "1d717853-18d5-4ea8-a23f-8a43bc21d45f";

    public static Id Route(ConnectHub _) => SingleInstanceId;
    public static Id Route(StartHubClient _) => SingleInstanceId;

    readonly ISubscriptionClient _client;
    bool _connecting;
    bool _connected;

    public HubTopic(ISubscriptionClient client) =>
        _client = client ?? throw new ArgumentNullException(nameof(client));

    protected void Given(HubConnecting _)
    {
        _connecting = true;
        _connected = false;
    }

    protected void Given(HubConnected _)
    {
        _connecting = false;
        _connected = true;
    }

    protected void When(ConnectHub command)
    {
        if(_connecting)
        {
            ThenError(SubscriptionErrors.HubAlreadyConnecting);
            return;
        }

        if(_connected)
        {
            ThenError(SubscriptionErrors.HubAlreadyConnected);
            return;
        }

        Then(new HubConnecting(command.AttemptsLeft));
    }

    protected async Task When(StartHubClient command, CancellationToken cancellationToken)
    {
        if(!_connecting)
        {
            ThenError(SubscriptionErrors.HubNotConnecting);
            return;
        }

        if(_connected)
        {
            ThenError(SubscriptionErrors.HubAlreadyConnected);
            return;
        }

        try
        {
            var connectionId = await _client.StartHubAsync(cancellationToken);

            Then(new HubConnected(connectionId));
        }
        catch(Exception error)
        {
            Then(new StartHubClientFailed(error.ToString(), command.AttemptsLeft - 1));
        }
    }
}
