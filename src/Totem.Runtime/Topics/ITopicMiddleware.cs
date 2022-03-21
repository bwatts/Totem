using Totem.Core;

namespace Totem.Topics;

public interface ITopicMiddleware
{
    Task InvokeAsync(ITopicContext<ICommandMessage> context, Func<Task> next, CancellationToken cancellationToken);
}