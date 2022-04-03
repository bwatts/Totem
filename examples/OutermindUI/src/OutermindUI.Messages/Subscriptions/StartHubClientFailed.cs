namespace OutermindUI.Subscriptions;

public class StartHubClientFailed : IEvent
{
    public StartHubClientFailed(string error, int attemptsLeft)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
        AttemptsLeft = attemptsLeft >= 0 ? attemptsLeft : throw new ArgumentOutOfRangeException(nameof(attemptsLeft));
    }

    public string Error { get; }
    public int AttemptsLeft { get; }
}
