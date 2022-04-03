namespace Totem.Map.Summary;

public class EventHandlerSummary
{
    internal EventHandlerSummary(Id typeId, Id serviceTypeId)
    {
        TypeId = typeId;
        ServiceTypeId = serviceTypeId;
    }

    public Id TypeId { get; }
    public Id ServiceTypeId { get; }
}
