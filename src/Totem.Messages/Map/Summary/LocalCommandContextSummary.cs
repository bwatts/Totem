namespace Totem.Map.Summary;

public class LocalCommandContextSummary
{
    internal LocalCommandContextSummary(Id typeId, TopicWhenSummary? when)
    {
        TypeId = typeId;
        When = when;
    }

    public Id TypeId { get; }
    public TopicWhenSummary? When { get; }
}
