namespace Totem.Http.Commands;

public interface IHttpCommandNegotiator
{
    HttpRequestMessage Negotiate(IHttpCommand command, string contentType);
}
