namespace OutermindUI.Subscriptions;

public class HubConnecting : IEvent
{
    public HubConnecting(int attemptsLeft) =>
        AttemptsLeft = attemptsLeft >= 0 ? attemptsLeft : throw new ArgumentOutOfRangeException(nameof(attemptsLeft));

    public int AttemptsLeft { get; }
}
