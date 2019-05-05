using System;

namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Signals a metric was assigned a new value
  /// </summary>
  /// <typeparam name="T">The type of monitored value</typeparam>
  public class MetricWritten<T>
  {
    public MetricWritten(DateTimeOffset when, Metric<T> metric, MetricPath path, T value)
    {
      When = when;
      Metric = metric;
      Path = path;
      Value = value;
    }

    public readonly DateTimeOffset When;
    public readonly Metric<T> Metric;
    public readonly MetricPath Path;
    public readonly T Value;
  }
}