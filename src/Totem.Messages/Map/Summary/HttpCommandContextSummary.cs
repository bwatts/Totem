namespace Totem.Map.Summary;

public class HttpCommandContextSummary
{
    internal HttpCommandContextSummary(string href, Id typeId, HttpRequestSummary request, TopicRouteSummary? route, TopicWhenSummary? when)
    {
        Href = href;
        TypeId = typeId;
        Request = request;
        Route = route;
        When = when;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public HttpRequestSummary Request { get; }
    public TopicRouteSummary? Route { get; }
    public TopicWhenSummary? When { get; }
}
