namespace Totem.Map.Summary;

public class LocalCommandContextSummary
{
    internal LocalCommandContextSummary(string href, Id typeId, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public TopicWhenSummary? When { get; }
}
