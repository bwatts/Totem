namespace Totem.Http.Commands;

public interface IHttpCommandClientContext<out TCommand> : IMessageContext
    where TCommand : IHttpCommand
{
    new IHttpCommandEnvelope Envelope { get; }
    new HttpCommandInfo Info { get; }
    TCommand Command { get; }
    ItemKey CommandKey { get; }
    Type CommandType { get; }
    Id CommandId { get; }
    string ContentType { get; set; }
    HttpRequestHeaders Headers { get; }
    ClientResponse? Response { get; set; }
}
