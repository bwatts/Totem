namespace Totem.Map.Summary;

public class QueueCommandContextSummary
{
    internal QueueCommandContextSummary(Id typeId, string queueName, TopicWhenSummary? when)
    {
        TypeId = typeId;
        QueueName = queueName;
        When = when;
    }

    public Id TypeId { get; }
    public string QueueName { get; }
    public TopicWhenSummary? When { get; }
}
