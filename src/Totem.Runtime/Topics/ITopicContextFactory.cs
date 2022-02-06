using Totem.Core;

namespace Totem.Topics;

public interface ITopicContextFactory
{
    ITopicContext<ICommandMessage> Create(Id pipelineId, ICommandContext<ICommandMessage> context, ItemKey topicKey);
}
