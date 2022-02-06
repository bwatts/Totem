using System.Collections.Generic;
using Totem.Core;
using Totem.Queues;

namespace Totem.Workflows;

public interface IWorkflow : IObserverTimeline
{
    IEnumerable<IQueueCommandEnvelope> NewCommands { get; }
    bool HasNewCommands { get; }
}
