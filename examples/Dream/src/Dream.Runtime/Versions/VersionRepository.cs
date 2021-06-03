using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Totem;

namespace Dream.Versions
{
    public class VersionRepository : IVersionRepository
    {
        const string PartitionKey = "Versions";

        readonly IStorage _storage;

        public VersionRepository(IStorage storage) =>
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));

        public Task<VersionRecord?> GetAsync(Id versionId, CancellationToken cancellationToken) =>
            _storage.GetValueAsync<VersionRecord>(PartitionKey, versionId.ToString(), cancellationToken);

        public IAsyncEnumerable<VersionRecord> ListAsync(CancellationToken cancellationToken) =>
            from item in _storage.ListAsync<VersionRecord>(PartitionKey, cancellationToken)
            select item.Value;

        public Task SaveAsync(VersionRecord record, CancellationToken cancellationToken)
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