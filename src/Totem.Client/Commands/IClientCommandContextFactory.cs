using Totem.Core;

namespace Totem.Commands
{
    public interface IClientCommandContextFactory
    {
        IClientCommandContext<ICommand> Create(Id pipelineId, ICommandEnvelope envelope);
    }
}