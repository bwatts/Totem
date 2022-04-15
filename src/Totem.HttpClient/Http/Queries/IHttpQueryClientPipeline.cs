namespace Totem.Http.Queries;

public interface IHttpQueryClientPipeline
{
    Id Id { get; }

    Task<IHttpQueryClientContext<IHttpQuery>> RunAsync(IHttpQueryEnvelope envelope, CancellationToken token);
}
