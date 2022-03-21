namespace Totem.Map.Summary;

public class HttpCommandContextSummary
{
    internal HttpCommandContextSummary(string href, Id typeId, HttpRequestSummary request, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        Request = request;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public HttpRequestSummary Request { get; }
    public TopicWhenSummary? When { get; }
}
