using System.Threading;
using System.Threading.Tasks;
using Totem.Core;

namespace Totem.Topics;

public interface ITopicStore
{
    Task<ITopicTransaction> StartTransactionAsync(ITopicContext<ICommandMessage> context, CancellationToken cancellationToken);
    Task CommitAsync(ITopicTransaction transaction, CancellationToken cancellationToken);
    Task RollbackAsync(ITopicTransaction transaction, CancellationToken cancellationToken);
}
