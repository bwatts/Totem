namespace Totem.Map.Summary;

public class LocalQueryContextSummary
{
    internal LocalQueryContextSummary(string href, Id typeId, QueryResultSummary result)
    {
        Href = href;
        TypeId = typeId;
        Result = result;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public QueryResultSummary Result { get; }
}
