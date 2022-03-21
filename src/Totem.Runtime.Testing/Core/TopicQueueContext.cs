using System.Security.Claims;
using Totem.Queues;

namespace Totem.Core;

public class TopicQueueContext : TopicContext<IQueueCommand, IQueueCommandContext<IQueueCommand>>
{
    internal TopicQueueContext(ITopicTests tests) : base(tests)
    { }

    protected override IQueueCommandContext<IQueueCommand> CreateContext(IQueueCommand command, Id commandId, Id correlationId, ClaimsPrincipal principal)
    {
        var contextFactory = new QueueCommandContextFactory(Tests.RuntimeMap);
        var pipelineId = Id.NewId();
        var commandType = command.GetType();
        var commandKey = new ItemKey(commandType, commandId);
        var info = QueueCommandInfo.From(commandType);
        var envelope = new QueueCommandEnvelope(commandKey, command, info, correlationId, principal);

        return contextFactory.Create(pipelineId, envelope);
    }
}
