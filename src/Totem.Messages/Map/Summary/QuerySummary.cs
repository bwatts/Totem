namespace Totem.Map.Summary;

public class QuerySummary
{
    internal QuerySummary(Id typeId, HttpQueryContextSummary? httpContext, LocalQueryContextSummary? localContext)
    {
        TypeId = typeId;
        HttpContext = httpContext;
        LocalContext = localContext;
    }

    public Id TypeId { get; }
    public HttpQueryContextSummary? HttpContext { get; }
    public LocalQueryContextSummary? LocalContext { get; }
}
