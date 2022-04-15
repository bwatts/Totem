namespace Totem.Http.Queries;

public class HttpQueryClient : IHttpQueryClient
{
    readonly IHttpQueryClientPipeline _pipeline;

    public HttpQueryClient(IHttpQueryClientPipeline pipeline) =>
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));

    public Task<IHttpQueryClientContext<IHttpQuery>> SendAsync(IHttpQueryEnvelope envelope, CancellationToken cancellationToken) =>
        _pipeline.RunAsync(envelope, cancellationToken);
}
