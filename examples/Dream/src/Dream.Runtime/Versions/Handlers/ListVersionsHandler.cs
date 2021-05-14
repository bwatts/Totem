using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions.Handlers
{
    public class ListVersionsHandler : IQueryHandler<ListVersions>
    {
        readonly IVersionRepository _repository;

        public ListVersionsHandler(IVersionRepository repository) =>
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public async Task HandleAsync(IQueryContext<ListVersions> context, CancellationToken cancellationToken)
        {
            var versions = await _repository.ListAsync(cancellationToken).ToArrayAsync(cancellationToken);

            context.RespondOk(versions);
        }
    }
}