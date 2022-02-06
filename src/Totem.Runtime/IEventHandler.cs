namespace Totem;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken);
}
