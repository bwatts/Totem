using System;

namespace Totem.Timeline
{
  /// <summary>
  /// Identifies a distinct point on the timeline
  /// </summary>
  public struct TimelinePosition : IEquatable<TimelinePosition>, IComparable<TimelinePosition>
  {
    public static readonly TimelinePosition None = new TimelinePosition();

    readonly long? _point;

    public TimelinePosition(long? point) : this()
    {
      if(point != null && point < 0) throw new ArgumentOutOfRangeException(nameof(point));

      _point = point;
    }

    public bool IsNone => _point == null;
    public bool IsSome => _point != null;

    public override string ToString() =>
      "#" + (IsNone ? "-" : _point.ToString());

    public long ToInt64()
    {
      Expect.False(IsNone, "Position is before the start of the timeline");

      return _point.Value;
    }

    public long? ToInt64OrNull() =>
      _point;

    public TimelinePosition Next() =>
      new TimelinePosition(_point == null ? 0 : _point + 1);

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      obj is TimelinePosition && Equals((TimelinePosition) obj);

    public bool Equals(TimelinePosition other) =>
      Eq.Values(this, other).Check(x => x._point);

    public override int GetHashCode() =>
      _point == null ? 0 : _point.GetHashCode();

    public int CompareTo(TimelinePosition other) =>
      Cmp.Values(this, other).Check(x => x._point);

    public static bool operator ==(TimelinePosition x, TimelinePosition y) => Eq.Op(x, y);
    public static bool operator !=(TimelinePosition x, TimelinePosition y) => Eq.OpNot(x, y);
    public static bool operator >(TimelinePosition x, TimelinePosition y) => Cmp.Op(x, y) > 0;
    public static bool operator <(TimelinePosition x, TimelinePosition y) => Cmp.Op(x, y) < 0;
    public static bool operator >=(TimelinePosition x, TimelinePosition y) => Cmp.Op(x, y) >= 0;
    public static bool operator <=(TimelinePosition x, TimelinePosition y) => Cmp.Op(x, y) <= 0;
  }
}