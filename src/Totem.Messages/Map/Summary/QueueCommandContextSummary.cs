namespace Totem.Map.Summary;

public class QueueCommandContextSummary
{
    internal QueueCommandContextSummary(string href, Id typeId, string queueName, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        QueueName = queueName;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public string QueueName { get; }
    public TopicWhenSummary? When { get; }
}
