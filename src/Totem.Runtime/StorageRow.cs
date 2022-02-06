namespace Totem;

public static class StorageRow
{
    public static IStorageRow<T> From<T>(string partitionKey, T value) =>
        new StorageRow<T>(new StorageKey(partitionKey), value);

    public static IStorageRow<T> From<T>(string partitionKey, T value, DateTimeOffset? whenExpires) =>
        new StorageRow<T>(new StorageKey(partitionKey), value, whenExpires);

    public static IStorageRow<T> From<T>(string partitionKey, string rowKey, T value) =>
        new StorageRow<T>(new StorageKey(partitionKey, rowKey), value);

    public static IStorageRow<T> From<T>(string partitionKey, string rowKey, T value, DateTimeOffset? whenExpires) =>
        new StorageRow<T>(new StorageKey(partitionKey, rowKey), value, whenExpires);
}

public class StorageRow<T> : IStorageRow<T>
{
    public StorageRow(StorageKey key, T? value = default, DateTimeOffset? whenExpires = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
        WhenExpires = whenExpires;
    }

    public StorageKey Key { get; set; }
    public Type ValueType => typeof(T);
    public T? Value { get; set; }
    public DateTimeOffset? WhenExpires { get; set; }

    object? IStorageRow.Value => Value;

    public override string ToString() => $"{Key} ({typeof(T)})";
}
