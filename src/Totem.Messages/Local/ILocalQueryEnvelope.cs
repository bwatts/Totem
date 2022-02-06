using Totem.Core;

namespace Totem.Local;

public interface ILocalQueryEnvelope : ILocalMessageEnvelope, IQueryEnvelope
{
    new ILocalQuery Message { get; }
    new LocalQueryInfo Info { get; }
}
