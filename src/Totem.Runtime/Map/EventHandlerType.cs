namespace Totem.Map;

public class EventHandlerType : MapType
{
    internal EventHandlerType(Type declaredType, EventType e, Type serviceType) : base(declaredType)
    {
        Event = e;
        ServiceType = serviceType;
    }

    public EventType Event { get; }
    public Type ServiceType { get; }
}
