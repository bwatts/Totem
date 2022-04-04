namespace Totem.Map.Summary;

public class ReportRowSummary
{
    internal ReportRowSummary(Id typeId, IReadOnlyList<ReportRowPropertySummary> properties)
    {
        TypeId = typeId;
        Properties = properties;
    }

    public Id TypeId { get; }
    public IReadOnlyList<ReportRowPropertySummary> Properties { get; }
}
