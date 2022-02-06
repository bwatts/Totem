using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Totem;

public interface IStorage
{
    Task<IStorageRow<T>> CreateAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken);
    Task<IStorageRow?> GetAsync(StorageKey key, CancellationToken cancellationToken);
    Task<IStorageRow<T>?> GetAsync<T>(StorageKey key, CancellationToken cancellationToken);
    Task<IReadOnlyList<IStorageRow>> ListAsync(string partitionKey, CancellationToken cancellationToken);
    Task<IReadOnlyList<IStorageRow<T>>> ListAsync<T>(string partitionKey, CancellationToken cancellationToken);
    Task<IStorageRow<T>> PutAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken);
    Task<IStorageRow> PutAsync(IStorageRow row, CancellationToken cancellationToken);
    Task PutAsync<T>(IEnumerable<IStorageRow<T>> rows, CancellationToken cancellationToken);
    Task PutAsync(IEnumerable<IStorageRow> rows, CancellationToken cancellationToken);
    Task RemoveAsync(StorageKey key, CancellationToken cancellationToken);
    Task RemoveAsync(IEnumerable<StorageKey> keys, CancellationToken cancellationToken);
}
