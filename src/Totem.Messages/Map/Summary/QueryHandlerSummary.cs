using System.Collections.Generic;

namespace Totem.Map.Summary;

public class QueryHandlerSummary
{
    internal QueryHandlerSummary(string href, Id typeId, IReadOnlyList<Id> serviceTypeIds)
    {
        Href = href;
        TypeId = typeId;
        ServiceTypeIds = serviceTypeIds;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public IReadOnlyList<Id> ServiceTypeIds { get; }
}
