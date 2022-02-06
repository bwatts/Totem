namespace Totem.Map.Summary;

public class QueryResultSummary
{
    internal QueryResultSummary(string href, Id typeId, bool isReport, bool isSingleRow, bool isManyRows, Id? rowTypeId)
    {
        Href = href;
        TypeId = typeId;
        IsReport = isReport;
        IsSingleRow = isSingleRow;
        IsManyRows = isManyRows;
        RowTypeId = rowTypeId;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public bool IsReport { get; }
    public bool IsSingleRow { get; }
    public bool IsManyRows { get; }
    public Id? RowTypeId { get; }
}
