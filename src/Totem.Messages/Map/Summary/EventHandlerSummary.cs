namespace Totem.Map.Summary;

public class EventHandlerSummary
{
    internal EventHandlerSummary(string href, Id typeId, Id serviceTypeId)
    {
        Href = href;
        TypeId = typeId;
        ServiceTypeId = serviceTypeId;
    }

    public string Href { get; }
    public Id TypeId { get; }
    public Id ServiceTypeId { get; }
}
