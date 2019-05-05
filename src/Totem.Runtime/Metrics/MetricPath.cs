using System;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// The path of a particular value in a metric. Implicitly converts from
  /// <see cref="string"/> and <see cref="Totem.Id"/>.
  /// </summary>
  public struct MetricPath : IEquatable<MetricPath>
  {
    readonly string _path;

    public MetricPath(string path = "") : this()
    {
      _path = path;
    }

    public override string ToString() => _path ?? "";

    public override bool Equals(object obj) =>
      obj is MetricPath && Equals((MetricPath) obj);

    public bool Equals(MetricPath other) =>
      ToString().Equals(other.ToString());

    public override int GetHashCode() =>
      ToString().GetHashCode();

    public static bool operator ==(MetricPath x, MetricPath y) => Eq.Op(x, y);
    public static bool operator !=(MetricPath x, MetricPath y) => Eq.OpNot(x, y);

    public static implicit operator MetricPath(string path) => new MetricPath(path);
    public static implicit operator MetricPath(Id path) => new MetricPath(path.ToString());
  }
}