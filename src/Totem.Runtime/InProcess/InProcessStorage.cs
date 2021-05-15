using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Totem.InProcess
{
    public class InProcessStorage : IStorage
    {
        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IStorageRow>> _partitionsByKey = new();
        readonly ILogger _logger;

        public InProcessStorage(ILogger<InProcessStorage> logger) =>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public Task<IStorageRow<T>> CreateAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken)
        {
            if(row == null)
                throw new ArgumentNullException(nameof(row));

            _logger.LogTrace("Create key {RowKey}", row.Key);

            var partition = GetOrAddPartition(row.Key.Partition);

            if(!partition.TryAdd(row.Key.Row, row))
                throw new InvalidOperationException($"Row key is already present: {row.Key}");

            return Task.FromResult(row);
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

                if(row == null)
                    throw new InvalidOperationException($"Expected key {key} to have type {typeof(T)} but found {untypedRow.ValueType}");
            }

            return Task.FromResult(row);
        }

        public async IAsyncEnumerable<IStorageRow> ListAsync(string partitionKey, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(partitionKey))
                throw new ArgumentOutOfRangeException(nameof(partitionKey));

            await Task.Yield(); // Avoid dependency on System.Linq.Async for .ToAsyncEnumerable()

            if(_partitionsByKey.TryGetValue(partitionKey, out var partition))
            {
                foreach(var row in partition.Values)
                {
                    yield return row;
                }
            }
        }

        public async IAsyncEnumerable<IStorageRow<T>> ListAsync<T>(string partitionKey, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(partitionKey))
                throw new ArgumentOutOfRangeException(nameof(partitionKey));

            await Task.Yield();

            if(_partitionsByKey.TryGetValue(partitionKey, out var partition))
            {
                foreach(var untypedrow in partition.Values)
                {
                    if(untypedrow is not IStorageRow<T> row)
                        throw new InvalidOperationException($"Expected partition {partitionKey} to have rows of type {typeof(T)} but found {untypedrow.ValueType}");

                    yield return row;
                }
            }
        }

        public Task<IStorageRow<T>> PutAsync<T>(IStorageRow<T> row, CancellationToken cancellationToken)
        {
            if(row == null)
                throw new ArgumentNullException(nameof(row));

            _logger.LogTrace("Put storage key {RowKey}", row.Key);

            var partition = GetOrAddPartition(row.Key.Partition);

            partition[row.Key.Row] = row;

            return Task.FromResult(row);
        }

        public Task<IStorageRow> PutAsync(IStorageRow row, CancellationToken cancellationToken)
        {
            if(row == null)
                throw new ArgumentNullException(nameof(row));

            _logger.LogTrace("Put storage key {RowKey}", row.Key);

            var partition = GetOrAddPartition(row.Key.Partition);

            partition[row.Key.Row] = row;

            return Task.FromResult(row);
        }

        public async IAsyncEnumerable<IStorageRow<T>> PutAsync<T>(IEnumerable<IStorageRow<T>> rows, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(rows == null)
                throw new ArgumentNullException(nameof(rows));

            await Task.Yield();

            foreach(var partitionRows in rows.GroupBy(row => row.Key.Partition))
            {
                var partition = GetOrAddPartition(partitionRows.Key);

                foreach(var partitionRow in partitionRows)
                {
                    _logger.LogTrace("Put storage key {RowKey}", partitionRow.Key);

                    partition[partitionRow.Key.Row] = partitionRow;

                    yield return partitionRow;
                }
            }
        }

        public async IAsyncEnumerable<IStorageRow> PutAsync(IEnumerable<IStorageRow> rows, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if(rows == null)
                throw new ArgumentNullException(nameof(rows));

            await Task.Yield();

            foreach(var partitionRows in rows.GroupBy(row => row.Key.Partition))
            {
                var partition = GetOrAddPartition(partitionRows.Key);

                foreach(var partitionRow in partitionRows)
                {
                    _logger.LogTrace("Put storage key {RowKey}", partitionRow.Key);

                    partition[partitionRow.Key.Row] = partitionRow;

                    yield return partitionRow;
                }
            }
        }

        public Task RemoveAsync(StorageKey key, CancellationToken cancellationToken)
        {
            if(key is null)
                throw new ArgumentNullException(nameof(key));

            _logger.LogTrace("Remove key {RowKey}", key);

            if(_partitionsByKey.TryGetValue(key.Partition, out var partition))
            {
                partition.Remove(key.Row, out _);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<StorageKey> keys, CancellationToken cancellationToken)
        {
            if(keys == null)
                throw new ArgumentNullException(nameof(keys));

            foreach(var key in keys)
            {
                _logger.LogTrace("Remove key {RowKey}", key);

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
}