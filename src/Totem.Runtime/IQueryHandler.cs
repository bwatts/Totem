using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public interface IQueryHandler<in TQuery> where TQuery : IQuery
    {
        Task HandleAsync(IQueryContext<TQuery> context, CancellationToken cancellationToken);
    }
}