using Totem.Core;
using Totem.Local;

namespace Totem;

public interface ILocalCommandContext<out TCommand> : ICommandContext<TCommand>
    where TCommand : ILocalCommand
{
    new ILocalCommandEnvelope Envelope { get; }
    new LocalCommandInfo Info { get; }
}
