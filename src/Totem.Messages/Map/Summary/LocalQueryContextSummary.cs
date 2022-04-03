namespace Totem.Map.Summary;

public class LocalQueryContextSummary
{
    internal LocalQueryContextSummary(Id typeId, QueryResultSummary result)
    {
        TypeId = typeId;
        Result = result;
    }

    public Id TypeId { get; }
    public QueryResultSummary Result { get; }
}
