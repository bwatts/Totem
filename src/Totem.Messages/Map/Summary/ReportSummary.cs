namespace Totem.Map.Summary;

public class ReportSummary
{
    internal ReportSummary(string href, Id typeId, IReadOnlyList<ObservationSummary> observations, IReadOnlyList<Id> queryTypeIds)
    {
        Href = href;
        TypeId = typeId;
        Observations = observations;
        QueryTypeIds = queryTypeIds;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public IReadOnlyList<ObservationSummary> Observations { get; }
    public IReadOnlyList<Id> QueryTypeIds { get; }
}
