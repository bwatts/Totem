namespace OutermindUI.Subscriptions.Reports;

public class HubReport : Report<HubRow>
{
    public static Id SingleInstanceId => (Id) "07a92836-14b5-4e2f-8e8e-aa7669135de6";

    public static Id Route(HubConnecting _) => SingleInstanceId;
    public static Id Route(HubConnected _) => SingleInstanceId;
    public static Id Route(StartHubClientFailed _) => SingleInstanceId;

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
