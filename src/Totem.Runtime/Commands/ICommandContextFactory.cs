using Totem.Core;

namespace Totem.Commands
{
    public interface ICommandContextFactory
    {
        ICommandContext<ICommand> Create(Id pipelineId, ICommandEnvelope envelope);
    }
}