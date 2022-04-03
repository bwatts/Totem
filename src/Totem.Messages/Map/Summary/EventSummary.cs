namespace Totem.Map.Summary;

public class EventSummary
{
    internal EventSummary(Id typeId, IReadOnlyList<Id> reportTypeIds, IReadOnlyList<Id> workflowTypeIds, IReadOnlyList<Id> handlerTypeIds)
    {
        TypeId = typeId;
        ReportTypeIds = reportTypeIds;
        WorkflowTypeIds = workflowTypeIds;
        HandlerTypeIds = handlerTypeIds;
    }

    public Id TypeId { get; }
    public IReadOnlyList<Id> ReportTypeIds { get; }
    public IReadOnlyList<Id> WorkflowTypeIds { get; }
    public IReadOnlyList<Id> HandlerTypeIds { get; }
}
