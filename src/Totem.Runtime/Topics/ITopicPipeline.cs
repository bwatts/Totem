using Totem.Core;

namespace Totem.Topics;

public interface ITopicPipeline
{
    Id Id { get; }

    Task<ITopicContext<ICommandMessage>> RunAsync(ICommandContext<ICommandMessage> commandContext, ItemKey topicKey, CancellationToken cancellationToken);
}
