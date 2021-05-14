using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Totem
{
    public static class StorageExtensions
    {
        public static Task<IStorageRow<T>> CreateAsync<T>(this IStorage storage, string partitionKey, T value, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.CreateAsync(StorageRow.From(partitionKey, value), cancellationToken);
        }

        public static Task<IStorageRow<T>> CreateAsync<T>(this IStorage storage, string partitionKey, T value, DateTimeOffset? whenExpires, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.CreateAsync(StorageRow.From(partitionKey, value, whenExpires), cancellationToken);
        }

        public static Task<IStorageRow<T>> CreateAsync<T>(this IStorage storage, string partitionKey, string rowKey, T value, DateTimeOffset? whenExpires, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.CreateAsync(StorageRow.From(partitionKey, rowKey, value, whenExpires), cancellationToken);
        }

        public static Task<IStorageRow<T>?> GetAsync<T>(this IStorage storage, string partitionKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.GetAsync<T>(new StorageKey(partitionKey), cancellationToken);
        }

        public static Task<IStorageRow<T>?> GetAsync<T>(this IStorage storage, string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.GetAsync<T>(new StorageKey(partitionKey, rowKey), cancellationToken);
        }

        public static async Task<T?> GetValueAsync<T>(this IStorage storage, StorageKey key, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            var row = await storage.GetAsync<T>(key, cancellationToken);

            return row == null ? default : row.Value;
        }

        public static Task<T?> GetValueAsync<T>(this IStorage storage, string partitionKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.GetValueAsync<T>(new StorageKey(partitionKey), cancellationToken);
        }

        public static Task<T?> GetValueAsync<T>(this IStorage storage, string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.GetValueAsync<T>(new StorageKey(partitionKey, rowKey), cancellationToken);
        }

        public static Task<IStorageRow<T>> PutAsync<T>(this IStorage storage, string partitionKey, T value, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.PutAsync(StorageRow.From(partitionKey, value), cancellationToken);
        }

        public static Task<IStorageRow<T>> PutAsync<T>(this IStorage storage, string partitionKey, T value, DateTimeOffset? whenExpires, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.PutAsync(StorageRow.From(partitionKey, value, whenExpires), cancellationToken);
        }

        public static Task<IStorageRow<T>> PutAsync<T>(this IStorage storage, string partitionKey, string rowKey, T value, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.PutAsync(StorageRow.From(partitionKey, rowKey, value), cancellationToken);
        }

        public static Task<IStorageRow<T>> PutAsync<T>(this IStorage storage, string partitionKey, string rowKey, T value, DateTimeOffset? whenExpires, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.PutAsync(StorageRow.From(partitionKey, rowKey, value, whenExpires), cancellationToken);
        }

        public static IAsyncEnumerable<IStorageRow<T>> PutAsync<T>(this IStorage storage, IEnumerable<string> partitionKeys, T value, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            var rows = partitionKeys.Select(partitionKey => StorageRow.From(partitionKey, value));

            return storage.PutAsync(rows, cancellationToken);
        }

        public static IAsyncEnumerable<IStorageRow<T>> PutAsync<T>(this IStorage storage, IEnumerable<string> partitionKeys, string rowKey, T value, DateTimeOffset? whenExpires, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            var rows = partitionKeys.Select(partitionKey => StorageRow.From(partitionKey, rowKey, value, whenExpires));

            return storage.PutAsync(rows, cancellationToken);
        }

        public static Task RemoveAsync(this IStorage storage, string partitionKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.RemoveAsync(new StorageKey(partitionKey), cancellationToken);
        }

        public static Task RemoveAsync(this IStorage storage, string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            return storage.RemoveAsync(new StorageKey(partitionKey, rowKey), cancellationToken);
        }

        public static Task RemoveAsync(this IStorage storage, IEnumerable<string> partitionKeys, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            var keys = partitionKeys.Select(partitionKey => new StorageKey(partitionKey));

            return storage.RemoveAsync(keys, cancellationToken);
        }

        public static Task RemoveAsync(this IStorage storage, IEnumerable<string> partitionKeys, string rowKey, CancellationToken cancellationToken)
        {
            if(storage == null)
                throw new ArgumentNullException(nameof(storage));

            var keys = partitionKeys.Select(partitionKey => new StorageKey(partitionKey, rowKey));

            return storage.RemoveAsync(keys, cancellationToken);
        }
    }
}