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

    public static bool TryFrom(string value, AreaMap map, out QueryETag etag)
    {
      etag = null;

      var parts = value.Split('@');

      if(parts.Length > 0 && FlowKey.TryFrom(parts[0], map, out var key))
      {
        var checkpoint = parts.Length == 2 && long.TryParse(parts[1], out var position)
          ? new TimelinePosition(position)
          : TimelinePosition.None;

        etag = new QueryETag(key, checkpoint);
      }

      return etag != null;
    }

    public static QueryETag From(FlowKey key, TimelinePosition checkpoint) =>
      new QueryETag(key, checkpoint);

    public static QueryETag From(string value, AreaMap map)
    {
      if(!TryFrom(value, map, out var etag))
      {
        throw new FormatException($"Failed to parse query ETag: \"{value}\"");
      }

      return etag;
    }
  }
}