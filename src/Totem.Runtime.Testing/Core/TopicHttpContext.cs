using System.Security.Claims;
using Totem.Http;

namespace Totem.Core;

public class TopicHttpContext : TopicContext<IHttpCommand, IHttpCommandContext<IHttpCommand>>
{
    internal TopicHttpContext(ITopicTests tests) : base(tests)
    { }

    protected override IHttpCommandContext<IHttpCommand> CreateContext(IHttpCommand command, Id commandId, Id correlationId, ClaimsPrincipal principal)
    {
        var contextFactory = new HttpCommandContextFactory(Tests.RuntimeMap);
        var pipelineId = Id.NewId();
        var commandType = command.GetType();
        var commandKey = new ItemKey(commandType, commandId);
        var info = HttpCommandInfo.From(commandType);
        var envelope = new HttpCommandEnvelope(commandKey, command, info, correlationId, principal);

        return contextFactory.Create(pipelineId, envelope);
    }
}
