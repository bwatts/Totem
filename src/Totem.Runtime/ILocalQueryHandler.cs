using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface ILocalQueryHandler<in TQuery> where TQuery : ILocalQuery
    {
        Task HandleAsync(ILocalQueryContext<TQuery> context, CancellationToken cancellationToken);
    }
}