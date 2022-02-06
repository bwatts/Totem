using System.Collections.Generic;

namespace Totem.Map.Summary;

public class TopicSummary
{
    internal TopicSummary(string href, Id typeId, IReadOnlyList<Id> commandTypeIds, IReadOnlyList<GivenSummary> givens)
    {
        Href = href;
        TypeId = typeId;
        CommandTypeIds = commandTypeIds;
        Givens = givens;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public IReadOnlyList<Id> CommandTypeIds { get; }
    public IReadOnlyList<GivenSummary> Givens { get; }
}
