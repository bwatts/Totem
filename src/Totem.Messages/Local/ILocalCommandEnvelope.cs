using Totem.Core;

namespace Totem.Local;

public interface ILocalCommandEnvelope : ILocalMessageEnvelope, ICommandEnvelope
{
    new ILocalCommand Message { get; }
    new LocalCommandInfo Info { get; }
}
