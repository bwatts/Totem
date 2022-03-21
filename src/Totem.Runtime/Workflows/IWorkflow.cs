using Totem.Core;
using Totem.Queues;

namespace Totem.Workflows;

public interface IWorkflow : IObserverTimeline
{
    bool HasNewCommands { get; }

    IReadOnlyList<IQueueCommandEnvelope> TakeNewCommands();
}
