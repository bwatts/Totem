namespace OutermindUI.Subscriptions;

public class ConnectHub : IQueueCommand
{
    public ConnectHub(int attemptsLeft) =>
        AttemptsLeft = attemptsLeft >= 0 ? attemptsLeft : throw new ArgumentOutOfRangeException(nameof(attemptsLeft));

    public int AttemptsLeft { get; }
}
