using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions
{
    public interface IVersionRepository
    {
        Task<VersionRecord?> GetAsync(Id versionId, CancellationToken cancellationToken);
        IAsyncEnumerable<VersionRecord> ListAsync(CancellationToken cancellationToken);
        Task SaveAsync(VersionRecord record, CancellationToken cancellationToken);
        Task RemoveAsync(Id versionId, CancellationToken cancellationToken);
    }
}