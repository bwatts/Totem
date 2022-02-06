using Totem.Core;

namespace Totem.Map;

public class EventType : MessageType
{
    internal EventType(EventInfo info) : base(info.DeclaredType) =>
        Info = info;

    public EventInfo Info { get; }
    public TypeKeyedCollection<ReportType> Reports { get; } = new();
    public TypeKeyedCollection<WorkflowType> Workflows { get; } = new();
    public TypeKeyedCollection<EventHandlerType> Handlers { get; } = new();
}
