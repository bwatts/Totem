namespace Totem.Map.Summary;

public class LocalCommandContextSummary
{
    internal LocalCommandContextSummary(string href, Id typeId, TopicRouteSummary? route, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        Route = route;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public TopicRouteSummary? Route { get; }
    public TopicWhenSummary? When { get; }
}
