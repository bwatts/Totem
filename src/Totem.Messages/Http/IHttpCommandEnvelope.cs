namespace Totem.Http
{
    public interface IHttpCommandEnvelope : IHttpMessageEnvelope
    {
        new IHttpCommand Message { get; }
        new HttpCommandInfo Info { get; }
    }
}