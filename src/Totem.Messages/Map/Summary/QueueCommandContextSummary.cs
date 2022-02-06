namespace Totem.Map.Summary;

public class QueueCommandContextSummary
{
    internal QueueCommandContextSummary(string href, Id typeId, string queueName, TopicRouteSummary? route, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        QueueName = queueName;
        Route = route;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public string QueueName { get; }
    public TopicRouteSummary? Route { get; }
    public TopicWhenSummary? When { get; }
}
