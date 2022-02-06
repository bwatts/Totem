namespace Totem.Map.Summary;

public class CommandSummary
{
    internal CommandSummary(
        string href,
        Id typeId,
        HttpCommandContextSummary? httpContext,
        LocalCommandContextSummary? localContext,
        QueueCommandContextSummary? queueContext,
        TopicRouteSummary? anyContextRoute,
        TopicWhenSummary? anyContextWhen)
    {
        Href = href;
        TypeId = typeId;
        HttpContext = httpContext;
        LocalContext = localContext;
        QueueContext = queueContext;
        AnyContextRoute = anyContextRoute;
        AnyContextWhen = anyContextWhen;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public HttpCommandContextSummary? HttpContext { get; }
    public LocalCommandContextSummary? LocalContext { get; }
    public QueueCommandContextSummary? QueueContext { get; }
    public TopicRouteSummary? AnyContextRoute { get; }
    public TopicWhenSummary? AnyContextWhen { get; }
}
