using System.Threading;
using System.Threading.Tasks;

namespace Totem;

public interface IHttpQueryHandler<in TQuery> where TQuery : IHttpQuery
{
    Task HandleAsync(IHttpQueryContext<TQuery> context, CancellationToken cancellationToken);
}
