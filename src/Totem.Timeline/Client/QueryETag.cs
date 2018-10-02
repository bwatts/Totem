using System;
using Totem.Timeline.Area;

namespace Totem.Timeline.Client
{
  /// <summary>
  /// The key and position of a query on the timeline
  /// </summary>
  public sealed class QueryETag : IEquatable<QueryETag>
  {
    QueryETag(FlowKey key, TimelinePosition checkpoint)
    {
      Key = key;
      Checkpoint = checkpoint;
    }

    public readonly FlowKey Key;
    public readonly TimelinePosition Checkpoint;

    public QueryETag WithCheckpoint(TimelinePosition checkpoint) =>
      new QueryETag(Key, checkpoint);

    public QueryETag GetLatest(QueryETag other) =>
      Checkpoint > other.Checkpoint ? this : other;

    public override string ToString() =>
      Checkpoint.IsNone ? Key.ToString() : $"{Key}@{Checkpoint.ToInt64()}";

    public override bool Equals(object obj) =>
      Equals(obj as QueryETag);

    public bool Equals(QueryETag other) =>
      Eq.Values(this, other).Check(x => x.Key).Check(x => x.Checkpoint);

    public override int GetHashCode() =>
      HashCode.Combine(Key, Checkpoint);

    public static bool operator ==(QueryETag x, QueryETag y) => Eq.Op(x, y);
    public static bool operator !=(QueryETag x, QueryETag y) => Eq.OpNot(x, y);

    public static QueryETag From(FlowKey key, TimelinePosition position) =>
      new QueryETag(key, position);

    public static QueryETag From(AreaMap area, string value, bool strict = true)
    {
      var parts = value.Split('@');

      if(parts.Length > 0)
      {
        var key = FlowKey.From(area, parts[0], strict);

        if(key != null)
        {
          var checkpoint = parts.Length == 2 && long.TryParse(parts[1], out var position)
            ? new TimelinePosition(position)
            : TimelinePosition.None;

          return new QueryETag(key, checkpoint);
        }
      }

      Expect.False(strict, $"Failed to parse ETag: {value}");

      return null;
    }
  }
}