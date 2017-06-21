namespace Totem.Metrics
{
  /// <summary>
  /// Signals a metric was assigned a new value
  /// </summary>
  /// <typeparam name="T">The type of monitored value</typeparam>
  public class MetricWritten<T> : Event
  {
    public MetricWritten(Metric<T> metric, MetricPath path, T value)
    {
      Metric = metric;
      Path = path;
      Value = value;
    }

    public readonly Metric<T> Metric;
    public readonly MetricPath Path;
    public readonly T Value;
  }
}