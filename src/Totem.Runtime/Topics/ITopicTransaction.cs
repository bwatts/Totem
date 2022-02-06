using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Topics;

public interface ITopicTransaction
{
    ITopicContext<ICommandMessage> Context { get; }
    ITopic Topic { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
