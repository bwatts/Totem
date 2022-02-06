namespace Totem.Map.Summary;

public class HttpQueryContextSummary
{
    internal HttpQueryContextSummary(string href, Id typeId, HttpRequestSummary request, QueryResultSummary result)
    {
        Href = href;
        TypeId = typeId;
        Request = request;
        Result = result;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public HttpRequestSummary Request { get; }
    public QueryResultSummary Result { get; }
}
