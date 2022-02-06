using Totem.Http;

namespace Totem.Commands;

public interface IClientCommandPipeline
{
    Id Id { get; }

    Task<IClientCommandContext<IHttpCommand>> RunAsync(IHttpCommandEnvelope envelope, CancellationToken token);
}
