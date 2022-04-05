namespace OutermindUI.Subscriptions.Reports;

public class HubReport : Report<HubRow>
{
    protected void When(HubConnecting e)
    {
        Row.Connecting = true;
        Row.Connected = false;
        Row.StartError = null;
        Row.AttemptsLeft = e.AttemptsLeft;
    }

    protected void When(HubConnected _)
    {
        Row.Connecting = false;
        Row.Connected = true;
        Row.AttemptsLeft = 0;
    }

    protected void When(StartHubClientFailed e)
    {
        Row.Connecting = e.AttemptsLeft > 0;
        Row.Connected = false;
        Row.StartError = e.Error;
        Row.AttemptsLeft = e.AttemptsLeft;
    }
}
