using Totem.Core;

namespace Totem.Events;

public interface IEventPipeline
{
    Id Id { get; }

    Task<IEventContext<IEvent>> RunAsync(IEventEnvelope point, CancellationToken cancellationToken);
}
