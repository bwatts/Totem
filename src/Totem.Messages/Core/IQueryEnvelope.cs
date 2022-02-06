namespace Totem.Core;

public interface IQueryEnvelope : IMessageEnvelope
{
    new IQueryMessage Message { get; }
    new QueryInfo Info { get; }
}
