namespace Totem.Core;

public interface IEventEnvelope : IMessageEnvelope
{
    new IEvent Message { get; }
    new EventInfo Info { get; }
    DateTimeOffset WhenOccurred { get; }
}
