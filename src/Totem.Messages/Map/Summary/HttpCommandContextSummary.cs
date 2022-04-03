namespace Totem.Map.Summary;

public class HttpCommandContextSummary
{
    internal HttpCommandContextSummary(Id typeId, HttpRequestSummary request, TopicWhenSummary? when)
    {
        TypeId = typeId;
        Request = request;
        When = when;
    }

    public Id TypeId { get; }
    public HttpRequestSummary Request { get; }
    public TopicWhenSummary? When { get; }
}
