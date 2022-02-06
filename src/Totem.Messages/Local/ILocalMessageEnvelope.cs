using Totem.Core;

namespace Totem.Local;

public interface ILocalMessageEnvelope : IMessageEnvelope
{
    new ILocalMessage Message { get; }
    new ILocalMessageInfo Info { get; }
}
