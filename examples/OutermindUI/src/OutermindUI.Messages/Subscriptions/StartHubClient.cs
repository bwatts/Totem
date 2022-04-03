namespace OutermindUI.Subscriptions;

public class StartHubClient : IQueueCommand
{
    public StartHubClient(int attemptsLeft) =>
        AttemptsLeft = attemptsLeft > 0 ? attemptsLeft : throw new ArgumentOutOfRangeException(nameof(attemptsLeft));

    public int AttemptsLeft { get; }
}
