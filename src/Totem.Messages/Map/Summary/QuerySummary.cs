namespace Totem.Map.Summary;

public class QuerySummary
{
    internal QuerySummary(string href, Id typeId, HttpQueryContextSummary? httpContext, LocalQueryContextSummary? localContext)
    {
        Href = href;
        TypeId = typeId;
        HttpContext = httpContext;
        LocalContext = localContext;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public HttpQueryContextSummary? HttpContext { get; }
    public LocalQueryContextSummary? LocalContext { get; }
}
