namespace Totem.Http;

public interface IHttpQueryPipeline
{
    Id Id { get; }

    Task<IHttpQueryContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken token);
}
