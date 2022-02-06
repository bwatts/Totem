using System.Net.Http;

namespace Totem.Commands;

public interface IClientCommandNegotiator
{
    HttpRequestMessage Negotiate(IHttpCommand command, string contentType);
}
