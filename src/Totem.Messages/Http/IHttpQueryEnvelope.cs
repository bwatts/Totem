using Totem.Core;

namespace Totem.Http;

public interface IHttpQueryEnvelope : IHttpMessageEnvelope, IQueryEnvelope
{
    new IHttpQuery Message { get; }
    new HttpQueryInfo Info { get; }
}
