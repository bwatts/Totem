using System.Diagnostics.CodeAnalysis;

namespace Totem.Core;

public class TimelinePosition : IEquatable<TimelinePosition>, IComparable<TimelinePosition>
{
    TimelinePosition(ItemKey key, long index)
    {
        Key = key;
        Index = index;
    }

    public ItemKey Key { get; }
    public long Index { get; }

    public override string ToString() => $"{Key}@{Index}";
    public override int GetHashCode() => HashCode.Combine(Key, Index);
    public override bool Equals(object? obj) => Equals(obj as TimelinePosition);

    public bool Equals(TimelinePosition? other) =>
        other is not null && Key == other.Key && Index == other.Index;

    public int CompareTo(TimelinePosition? other)
    {
        if(other is null)
        {
            return 1;
        }

        var nameResult = Key.CompareTo(other.Key);

        return nameResult != 0 ? nameResult : Index.CompareTo(other.Index);
    }

    public static bool TryFrom(ItemKey timelineKey, long index, [NotNullWhen(true)] out TimelinePosition? position)
    {
        if(timelineKey is null || index < 0)
        {
            position = null;
            return false;
        }

        position = new(timelineKey, index);
        return true;
    }

    public static TimelinePosition From(ItemKey timelineKey, long index = 0)
    {
        if(timelineKey is null)
            throw new ArgumentNullException(nameof(timelineKey));

        if(index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index is negative: {index}");

        return new(timelineKey, index);
    }

    public static bool operator ==(TimelinePosition? x, TimelinePosition? y) => EqualityComparer<TimelinePosition>.Default.Equals(x, y);
    public static bool operator !=(TimelinePosition? x, TimelinePosition? y) => !EqualityComparer<TimelinePosition>.Default.Equals(x, y);
    public static bool operator <(TimelinePosition? x, TimelinePosition? y) => Comparer<TimelinePosition>.Default.Compare(x, y) < 0;
    public static bool operator >(TimelinePosition? x, TimelinePosition? y) => Comparer<TimelinePosition>.Default.Compare(x, y) > 0;
    public static bool operator <=(TimelinePosition? x, TimelinePosition? y) => Comparer<TimelinePosition>.Default.Compare(x, y) <= 0;
    public static bool operator >=(TimelinePosition? x, TimelinePosition? y) => Comparer<TimelinePosition>.Default.Compare(x, y) >= 0;
}
