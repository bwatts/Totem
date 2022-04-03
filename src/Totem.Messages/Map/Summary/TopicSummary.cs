namespace Totem.Map.Summary;

public class TopicSummary
{
    internal TopicSummary(Id typeId, IReadOnlyList<Id> commandTypeIds, IReadOnlyList<GivenSummary> givens)
    {
        TypeId = typeId;
        CommandTypeIds = commandTypeIds;
        Givens = givens;
    }

    public Id TypeId { get; }
    public IReadOnlyList<Id> CommandTypeIds { get; }
    public IReadOnlyList<GivenSummary> Givens { get; }
}
