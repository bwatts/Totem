using Totem.Core;

namespace Totem.Http
{
    public interface IHttpMessageEnvelope : IMessageEnvelope
    {
        new IHttpMessage Message { get; }
        new HttpMessageInfo Info { get; }
    }
}