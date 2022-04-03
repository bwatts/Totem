namespace OutermindUI.Subscriptions.Workflows;

public class ConnectSubscriptionsWorkflow : Workflow
{
    public static Id SingleInstanceId => (Id) "342c4967-0d0e-46f6-85cc-d1637967975a";

    public static Id Route(HubConnecting _) => SingleInstanceId;
    public static Id Route(StartHubClientFailed _) => SingleInstanceId;

    protected void When(HubConnecting _) =>
        ThenEnqueue(new StartHubClient(attemptsLeft: 3));

    protected void When(StartHubClientFailed e)
    {


        // TODO: Exponential backoff => ThenSchedule(...)


        if(e.AttemptsLeft > 0)
        {
            ThenEnqueue(new StartHubClient(e.AttemptsLeft));
        }
    }
}
