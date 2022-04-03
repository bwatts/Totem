namespace OutermindUI.Subscriptions;

public class HubRow : IReportRow
{
    public Id Id { get; set; } = null!;
    public bool Connecting { get; set; }
    public bool Connected { get; set; }
    public string? StartError { get; set; }
    public int AttemptsLeft { get; set; }
}
