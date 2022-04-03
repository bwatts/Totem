namespace Totem.Map.Summary;

public class QueryResultSummary
{
    internal QueryResultSummary(Id typeId, bool isReport, bool isSingleRow, bool isManyRows, Id? rowTypeId)
    {
        TypeId = typeId;
        IsReport = isReport;
        IsSingleRow = isSingleRow;
        IsManyRows = isManyRows;
        RowTypeId = rowTypeId;
    }

    public Id TypeId { get; }
    public bool IsReport { get; }
    public bool IsSingleRow { get; }
    public bool IsManyRows { get; }
    public Id? RowTypeId { get; }
}
