namespace Totem.Http
{
    public interface IHttpQueryEnvelope : IHttpMessageEnvelope
    {
        new IHttpQuery Message { get; }
        new HttpQueryInfo Info { get; }
    }
}