namespace Totem.Map.Summary;

public class EventSummary
{
    internal EventSummary(string href, Id typeId, IReadOnlyList<Id> reportTypeIds, IReadOnlyList<Id> workflowTypeIds, IReadOnlyList<Id> handlerTypeIds)
    {
        Href = href;
        TypeId = typeId;
        ReportTypeIds = reportTypeIds;
        WorkflowTypeIds = workflowTypeIds;
        HandlerTypeIds = handlerTypeIds;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public IReadOnlyList<Id> ReportTypeIds { get; }
    public IReadOnlyList<Id> WorkflowTypeIds { get; }
    public IReadOnlyList<Id> HandlerTypeIds { get; }
}
