using System.Net.Http;

namespace Totem.Commands
{
    public interface IClientCommandNegotiator
    {
        HttpRequestMessage Negotiate(ICommand command, string mediaType);
    }
}