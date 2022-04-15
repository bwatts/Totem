namespace Totem.Http.Commands;

public interface IHttpCommandClientPipeline
{
    Id Id { get; }

    Task<IHttpCommandClientContext<IHttpCommand>> RunAsync(IHttpCommandEnvelope envelope, CancellationToken token);
}
