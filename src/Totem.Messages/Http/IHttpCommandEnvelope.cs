using Totem.Core;

namespace Totem.Http;

public interface IHttpCommandEnvelope : IHttpMessageEnvelope, ICommandEnvelope
{
    new IHttpCommand Message { get; }
    new HttpCommandInfo Info { get; }
}
