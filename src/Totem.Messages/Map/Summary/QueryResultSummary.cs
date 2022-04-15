namespace Totem.Map.Summary;

public class QueryResultSummary
{
    internal QueryResultSummary(Id typeId, Id? reportRowTypeId, bool isSingleRow, bool isManyRows)
    {
        TypeId = typeId;
        ReportRowTypeId = reportRowTypeId;
        IsSingleRow = isSingleRow;
        IsManyRows = isManyRows;
    }

    public Id TypeId { get; }
    public Id? ReportRowTypeId { get; }
    public bool IsSingleRow { get; }
    public bool IsManyRows { get; }
}
