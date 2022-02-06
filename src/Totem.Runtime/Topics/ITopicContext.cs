using Totem.Core;
using Totem.Map;

namespace Totem.Topics;

public interface ITopicContext<out TCommand> where TCommand : ICommandMessage
{
    Id PipelineId { get; }
    ICommandContext<TCommand> CommandContext { get; }
    ItemKey TopicKey { get; }
    TopicType TopicType { get; }
}
