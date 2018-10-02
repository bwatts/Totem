namespace Totem.Runtime.Metrics
{
  /// <summary>
  /// Measures a discrete increasing quantity
  /// </summary>
  public class LongTotal : Metric<long>
  {
    public void Increment(MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + 1, path);

    public void IncrementBy(long count, MetricPath path = default(MetricPath)) =>
      AppendWrite(current => current + count, path);
  }
}