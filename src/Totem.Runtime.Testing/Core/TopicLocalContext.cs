using System.Security.Claims;
using Totem.Local;

namespace Totem.Core;

public class TopicLocalContext : TopicContext<ILocalCommand, ILocalCommandContext<ILocalCommand>>
{
    internal TopicLocalContext(ITopicTests tests) : base(tests)
    { }

    protected override ILocalCommandContext<ILocalCommand> CreateContext(ILocalCommand command, Id commandId, Id correlationId, ClaimsPrincipal principal)
    {
        var contextFactory = new LocalCommandContextFactory(Tests.RuntimeMap);
        var pipelineId = Id.NewId();
        var commandType = command.GetType();
        var commandKey = new ItemKey(commandType, commandId);
        var info = LocalCommandInfo.From(commandType);
        var envelope = new LocalCommandEnvelope(commandKey, command, info, correlationId, principal);

        return contextFactory.Create(pipelineId, envelope);
    }
}
