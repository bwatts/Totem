using Totem.Http;

namespace Totem.Commands;

public interface IClientCommandContextFactory
{
    IClientCommandContext<IHttpCommand> Create(Id pipelineId, IHttpCommandEnvelope envelope);
}
