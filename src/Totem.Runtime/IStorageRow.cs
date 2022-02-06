using System;

namespace Totem;

public interface IStorageRow
{
    StorageKey Key { get; }
    Type ValueType { get; }
    object? Value { get; }
    DateTimeOffset? WhenExpires { get; }
}

public interface IStorageRow<T> : IStorageRow
{
    new T? Value { get; }
}
