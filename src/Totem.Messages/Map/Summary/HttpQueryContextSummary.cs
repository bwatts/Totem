namespace Totem.Map.Summary;

public class HttpQueryContextSummary
{
    internal HttpQueryContextSummary(Id typeId, HttpRequestSummary request, QueryResultSummary result)
    {
        TypeId = typeId;
        Request = request;
        Result = result;
    }

    public Id TypeId { get; }
    public HttpRequestSummary Request { get; }
    public QueryResultSummary Result { get; }
}
