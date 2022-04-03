namespace Totem.Map.Summary;

public class ReportSummary
{
    internal ReportSummary(Id typeId, IReadOnlyList<ObservationSummary> observations, IReadOnlyList<Id> queryTypeIds)
    {
        TypeId = typeId;
        Observations = observations;
        QueryTypeIds = queryTypeIds;
    }

    public Id TypeId { get; }
    public IReadOnlyList<ObservationSummary> Observations { get; }
    public IReadOnlyList<Id> QueryTypeIds { get; }
}
