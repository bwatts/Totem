namespace Totem.Map.Summary;

public class QueryHandlerSummary
{
    internal QueryHandlerSummary(Id typeId, IReadOnlyList<Id> serviceTypeIds)
    {
        TypeId = typeId;
        ServiceTypeIds = serviceTypeIds;
    }

    public Id TypeId { get; }
    public IReadOnlyList<Id> ServiceTypeIds { get; }
}
