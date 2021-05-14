using System.Collections.Generic;
using Totem.Core;
using Totem.Routes;

namespace Totem.Workflows
{
    public interface IWorkflow : IRoute
    {
        bool HasNewCommands { get; }
        IEnumerable<IQueueEnvelope> NewCommands { get; }
    }
}