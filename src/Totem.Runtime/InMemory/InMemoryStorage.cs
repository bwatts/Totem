using System.Collections.Concurrent;

namespace Totem.InMemory;

public class InMemoryStorage : IStorage
{
    readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IStorageRow>> _partitionsByKey = new();
    readonly ILogger _logger;

    public InMemoryStorage(ILogger<InMemoryStorage> logger) =>
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public Task<IStorageRow<T>> CreateAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken)
    {
        if(row is null)
            throw new ArgumentNullException(nameof(row));

        _logger.LogTrace("[storage] Create key {RowKey}", row.Key);

        var partition = GetOrAddPartition(row.Key.Partition);

        if(!partition.TryAdd(row.Key.Row, row))
            throw new InvalidOperationException($"Row key is already present: {row.Key}");

        return Task.FromResult(row);
    }

    public Task<IStorageRow?> GetAsync(StorageKey key, CancellationToken cancellationToken)
    {
        if(key is null)
            throw new ArgumentNullException(nameof(key));

        if(_partitionsByKey.TryGetValue(key.Partition, out var partition) && partition.TryGetValue(key.Row, out var row))
        {
            return Task.FromResult<IStorageRow?>(row);
        }

        return Task.FromResult<IStorageRow?>(null);
    }

    public Task<IStorageRow<T>?> GetAsync<T>(StorageKey key, CancellationToken cancellationToken)
    {
        if(key is null)
            throw new ArgumentNullException(nameof(key));

        var row = default(IStorageRow<T>?);

        if(_partitionsByKey.TryGetValue(key.Partition, out var partition)
            && partition.TryGetValue(key.Row, out var untypedRow))
        {
            row = untypedRow as IStorageRow<T>;

            if(row is null)
                throw new InvalidOperationException($"Expected key {key} to have type {typeof(T)} but found {untypedRow.ValueType}");
        }

        return Task.FromResult(row);
    }

    public Task<IReadOnlyList<IStorageRow>> ListAsync(string partitionKey, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(partitionKey))
            throw new ArgumentOutOfRangeException(nameof(partitionKey));

        var rows = new List<IStorageRow>();

        if(_partitionsByKey.TryGetValue(partitionKey, out var partition))
        {
            foreach(var row in partition.Values)
            {
                rows.Add(row);
            }
        }

        return Task.FromResult<IReadOnlyList<IStorageRow>>(rows);
    }

    public Task<IReadOnlyList<IStorageRow<T>>> ListAsync<T>(string partitionKey, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(partitionKey))
            throw new ArgumentOutOfRangeException(nameof(partitionKey));

        var rows = new List<IStorageRow<T>>();

        if(_partitionsByKey.TryGetValue(partitionKey, out var partition))
        {
            foreach(var untypedrow in partition.Values)
            {
                if(untypedrow is not IStorageRow<T> row)
                    throw new InvalidOperationException($"Expected partition {partitionKey} to have rows of type {typeof(T)} but found {untypedrow.ValueType}");

                rows.Add(row);
            }
        }

        return Task.FromResult<IReadOnlyList<IStorageRow<T>>>(rows);
    }

    public Task<IStorageRow<T>> PutAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken)
    {
        if(row is null)
            throw new ArgumentNullException(nameof(row));

        _logger.LogTrace("[storage] Put key {RowKey}", row.Key);

        var partition = GetOrAddPartition(row.Key.Partition);

        partition[row.Key.Row] = row;

        return Task.FromResult(row);
    }

    public Task<IStorageRow> PutAsync(IStorageRow row, CancellationToken cancellationToken)
    {
        if(row is null)
            throw new ArgumentNullException(nameof(row));

        _logger.LogTrace("Put storage key {RowKey}", row.Key);

        var partition = GetOrAddPartition(row.Key.Partition);

        partition[row.Key.Row] = row;

        return Task.FromResult(row);
    }

    public async Task PutAsync<T>(IEnumerable<IStorageRow<T>> rows, CancellationToken cancellationToken)
    {
        if(rows is null)
            throw new ArgumentNullException(nameof(rows));

        await Task.Yield();

        foreach(var partitionRows in rows.GroupBy(row => row.Key.Partition))
        {
            var partition = GetOrAddPartition(partitionRows.Key);

            foreach(var partitionRow in partitionRows)
            {
                _logger.LogTrace("[storage] Put key {RowKey}", partitionRow.Key);

                partition[partitionRow.Key.Row] = partitionRow;
            }
        }
    }

    public async Task PutAsync(IEnumerable<IStorageRow> rows, CancellationToken cancellationToken)
    {
        if(rows is null)
            throw new ArgumentNullException(nameof(rows));

        await Task.Yield();

        foreach(var partitionRows in rows.GroupBy(row => row.Key.Partition))
        {
            var partition = GetOrAddPartition(partitionRows.Key);

            foreach(var partitionRow in partitionRows)
            {
                _logger.LogTrace("[storage] Put key {RowKey}", partitionRow.Key);

                partition[partitionRow.Key.Row] = partitionRow;
            }
        }
    }

    public Task RemoveAsync(StorageKey key, CancellationToken cancellationToken)
    {
        if(key is null)
            throw new ArgumentNullException(nameof(key));

        _logger.LogTrace("[storage] Remove key {RowKey}", key);

        if(_partitionsByKey.TryGetValue(key.Partition, out var partition))
        {
            partition.Remove(key.Row, out _);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(IEnumerable<StorageKey> keys, CancellationToken cancellationToken)
    {
        if(keys is null)
            throw new ArgumentNullException(nameof(keys));

        foreach(var key in keys)
        {
            _logger.LogTrace("[storage] Remove key {RowKey}", key);

            if(_partitionsByKey.TryGetValue(key.Partition, out var partition))
            {
                partition.Remove(key.Row, out _);
            }
        }

        return Task.CompletedTask;
    }

    ConcurrentDictionary<string, IStorageRow> GetOrAddPartition(string key) =>
        _partitionsByKey.GetOrAdd(key, _ => new());
}
