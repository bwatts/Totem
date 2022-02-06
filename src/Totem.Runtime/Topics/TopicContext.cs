using Totem.Core;
using Totem.Map;

namespace Totem.Topics;

public class TopicContext<TCommand> : MessageContext, ITopicContext<TCommand>
    where TCommand : class, ICommandMessage
{
    internal TopicContext(Id pipelineId, ICommandContext<TCommand> commandContext, ItemKey topicKey, TopicType topicType)
        : base(pipelineId, commandContext.Envelope)
    {
        CommandContext = commandContext;
        TopicKey = topicKey;
        TopicType = topicType;
    }

    public ICommandContext<TCommand> CommandContext { get; }
    public ItemKey TopicKey { get; }
    public TopicType TopicType { get; }
}
