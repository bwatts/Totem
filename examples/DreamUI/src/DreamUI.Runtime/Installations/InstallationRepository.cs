using System;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace DreamUI.Installations
{
    public class InstallationRepository : IInstallationRepository
    {
        const string PartitionKey = "Installations";

        readonly IStorage _storage;

        public InstallationRepository(IStorage storage) =>
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));

        public Task<InstallationRecord?> GetAsync(Id versionId, CancellationToken cancellationToken) =>
            _storage.GetValueAsync<InstallationRecord>(PartitionKey, versionId.ToString(), cancellationToken);

        public Task SaveAsync(InstallationRecord record, CancellationToken cancellationToken)
        {
            if(record == null)
                throw new ArgumentNullException(nameof(record));

            return _storage.PutAsync(PartitionKey, record.Id.ToString(), record, cancellationToken);
        }

        public Task RemoveAsync(Id versionId, CancellationToken cancellationToken)
        {
            if(versionId == null)
                throw new ArgumentNullException(nameof(versionId));

            return _storage.RemoveAsync(PartitionKey, versionId.ToString(), cancellationToken);
        }
    }
}