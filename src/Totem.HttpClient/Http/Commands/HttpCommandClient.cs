namespace Totem.Http.Commands;

public class HttpCommandClient : IHttpCommandClient
{
    readonly IHttpCommandClientPipeline _pipeline;

    public HttpCommandClient(IHttpCommandClientPipeline pipeline) =>
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));

    public Task<IHttpCommandClientContext<IHttpCommand>> SendAsync(IHttpCommandEnvelope envelope, CancellationToken cancellationToken) =>
        _pipeline.RunAsync(envelope, cancellationToken);
}
