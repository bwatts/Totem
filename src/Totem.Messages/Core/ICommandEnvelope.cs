namespace Totem.Core;

public interface ICommandEnvelope : IMessageEnvelope
{
    new ICommandMessage Message { get; }
    new CommandInfo Info { get; }
}
