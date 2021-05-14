using System;
using System.Collections.Generic;

namespace Totem
{
    public class StorageKey : IEquatable<StorageKey>
    {
        public const string RowSeparator = ":";
        public const string SingleRow = nameof(SingleRow);

        public StorageKey(string partition, string row = SingleRow)
        {
            if(string.IsNullOrWhiteSpace(partition))
                throw new ArgumentOutOfRangeException(nameof(partition));

            if(string.IsNullOrWhiteSpace(row))
                throw new ArgumentOutOfRangeException(nameof(row));

            Partition = partition;
            Row = row;
        }

        public string Partition { get; }
        public string Row { get; }
        public bool IsSingleRow => Row == SingleRow;

        public override string ToString() =>
            $"{Partition}{RowSeparator}{Row}";

        public bool Equals(StorageKey? other) =>
            other is StorageKey key && Partition == key.Partition && Row == key.Row;

        public override bool Equals(object? obj) =>
            Equals(obj as StorageKey);

        public override int GetHashCode() =>
            HashCode.Combine(Partition, Row);

        public static bool operator ==(StorageKey x, StorageKey y) => EqualityComparer<StorageKey>.Default.Equals(x, y);
        public static bool operator !=(StorageKey x, StorageKey y) => !(x == y);
    }
}