using System.Collections.Generic;

namespace Totem.Map.Summary;

public class WorkflowSummary
{
    internal WorkflowSummary(string href, Id typeId, IReadOnlyList<ObservationSummary> observations)
    {
        Href = href;
        TypeId = typeId;
        Observations = observations;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public IReadOnlyList<ObservationSummary> Observations { get; }
}
