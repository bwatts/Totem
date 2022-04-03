namespace Totem.Map.Summary;

public class CommandSummary
{
    internal CommandSummary(
        Id typeId,
        TopicRouteSummary route,
        TopicWhenSummary? whenWithoutContext,
        HttpCommandContextSummary? httpContext,
        LocalCommandContextSummary? localContext,
        QueueCommandContextSummary? queueContext)
    {
        TypeId = typeId;
        Route = route;
        WhenWithoutContext = whenWithoutContext;
        HttpContext = httpContext;
        LocalContext = localContext;
        QueueContext = queueContext;
    }

    public Id TypeId { get; }
    public TopicRouteSummary? Route { get; }
    public TopicWhenSummary? WhenWithoutContext { get; }
    public HttpCommandContextSummary? HttpContext { get; }
    public LocalCommandContextSummary? LocalContext { get; }
    public QueueCommandContextSummary? QueueContext { get; }
}
