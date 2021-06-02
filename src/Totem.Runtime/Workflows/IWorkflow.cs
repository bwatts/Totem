using System.Collections.Generic;
using Totem.Queues;
using Totem.Routes;

namespace Totem.Workflows
{
    public interface IWorkflow : IRoute
    {
        bool HasNewCommands { get; }
        IEnumerable<IQueueCommandEnvelope> NewCommands { get; }
    }
}